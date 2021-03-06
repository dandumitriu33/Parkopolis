﻿getCitiesFromDb();
populateUserDopdowns();

// USER MANAGEMENT

async function populateUserDopdowns() {
    $("#selectUserCityAdmin").empty();
    $("#selectUserAddParkingLot").empty();
    $("#selectUserCityAdmin").append(`<option value="" disabled selected hidden>User</option>`);
    $("#selectUserAddParkingLot").append(`<option value="" disabled selected hidden>User</option>`);
    await $.getJSON('http://localhost:1234/api/users', function (data) {
        let userRank = "";
        for (var i = 0; i < data.length; i++) {
            if (data[i].rank == 0) {
                userRank = "Consumer";
            }
            else if (data[i].rank == 1) {
                userRank = "Parking Lot Owner";
            }
            else if (data[i].rank == 2) {
                userRank = "Admin";
            }
            else {
                userRank = "Unknown";
            }
            let element = `<option value="${data[i].id}">${data[i].firstName} ${data[i].lastName} (${userRank}) (${data[i].id})</option>`;
            $("#selectUserCityAdmin").append(element);
            $("#selectUserAddParkingLot").append(element);
        }
    });
};

// make user Admin
$("#makeUserAdmin").click(async function (event) {
    event.preventDefault();
    let adminRank = 2;
    let userId = $("#selectUserCityAdmin").val();
    let tempUser = {
        "id": "",
        "firstName": "",
        "lastName": "",
        "rank": 0
    };
    await $.getJSON(`http://localhost:1234/api/users/${userId}`, async function (data) {
        tempUser.id = data.id;
        tempUser.firstName = data.firstName;
        tempUser.lastName = data.lastName;
        tempUser.rank = adminRank;
    })
    //alert(`make admin pressed ${tempUser.firstName}`);
    await $.ajax({
        type: "POST",
        url: `http://localhost:1234/api/users/${userId}`,
        data: JSON.stringify(tempUser),
        contentType: "application/json; charset=utf-8",
        crossDomain: true,
        dataType: "json",
        success: function () {
            console.log("User promoted to Admin successfully.");
        },
        error: function (jqXHR, status) {
            console.log(jqXHR);
            console.log('fail' + status.code);
        }
    })
    populateUserDopdowns();
})

// make user Parking Lot Owner
$("#makeUserParkinglotOwner").click(async function (event) {
    event.preventDefault();
    let parkingLotOwnerRank = 1;
    let userId = $("#selectUserCityAdmin").val();
    let tempUser = {
        "id": "",
        "firstName": "",
        "lastName": "",
        "rank": 0
    };
    await $.getJSON(`http://localhost:1234/api/users/${userId}`, async function (data) {
        tempUser.id = data.id;
        tempUser.firstName = data.firstName;
        tempUser.lastName = data.lastName;
        tempUser.rank = parkingLotOwnerRank;
    })
    await $.ajax({
        type: "POST",
        url: `http://localhost:1234/api/users/${userId}`,
        data: JSON.stringify(tempUser),
        contentType: "application/json; charset=utf-8",
        crossDomain: true,
        dataType: "json",
        success: function () {
            console.log("User promoted to Parking Lot Owner successfully.");
        },
        error: function (jqXHR, status) {
            console.log(jqXHR);
            console.log('fail' + status.code);
        }
    })
    populateUserDopdowns();
})

// make user Consumer / Customer
$("#makeUserConsumer").click(async function (event) {
    event.preventDefault();
    let consumerRank = 0;
    let userId = $("#selectUserCityAdmin").val();
    let tempUser = {
        "id": "",
        "firstName": "",
        "lastName": "",
        "rank": 0
    };
    await $.getJSON(`http://localhost:1234/api/users/${userId}`, async function (data) {
        tempUser.id = data.id;
        tempUser.firstName = data.firstName;
        tempUser.lastName = data.lastName;
        tempUser.rank = consumerRank;
    })
    await $.ajax({
        type: "POST",
        url: `http://localhost:1234/api/users/${userId}`,
        data: JSON.stringify(tempUser),
        contentType: "application/json; charset=utf-8",
        crossDomain: true,
        dataType: "json",
        success: function () {
            console.log("User promoted to Customer successfully.");
        },
        error: function (jqXHR, status) {
            console.log(jqXHR);
            console.log('fail' + status.code);
        }
    })
    populateUserDopdowns();
})


// CITY, AREA, PARKING LOT MANAGEMENT

// Submitting a new city
$("#newCitySubmit").click(async function () {
    console.log($("#cityName").val());
    let newCityName = $("#cityName").val();
    let data = { "name": newCityName};
    console.log(data);
    
    await $.ajax({
        type: "POST",
        url: "http://localhost:1028/api/cities",
        data: JSON.stringify(data),
        contentType: "application/json; charset=utf-8",
        crossDomain: true,
        dataType: "json",
        success: function () {
            console.log("City added successfully.");
        },
        error: function (jqXHR, status) {
            console.log(jqXHR);
            console.log('fail' + status.code);
        }
    })
    await $.getJSON(`http://localhost:1028/api/cities`, async function (data) {
        for (var i = 0; i < data.length; i++) {
            allCities.push(data[i]);
        }
        $("#citiesContainer").empty();
        populateCities();
        allCities = [];
    });
});

// Submitting a new Area in a City
$("#newAreaSubmit").click(async function () {
    let cityId = $("#selectCityAddArea").val();
    let newAreaName = await $("#areaNameAddArea").val();

    let data = {

        "name": newAreaName,
        "cityId": cityId,
    }

    $.ajax({
        type: "POST",
        url: `http://localhost:1028/api/cities/${cityId}/areas`,
        data: JSON.stringify(data),
        contentType: "application/json; charset=utf-8",
        crossDomain: true,
        dataType: "json",
        success: function () {
            console.log("Area added successfully.");
        },
        error: function (jqXHR, status) {
            // error handler
            console.log(jqXHR);
            console.log('fail' + status.code);
        }
    })
});

// Submitting a new Parking Lot based on Area and City
$("#selectCityAddParkingLot").change(function () {
    populateAreasDropdownAddParkingLot(); 
});

async function populateAreasDropdownAddParkingLot() {
    let cityId = $("#selectCityAddParkingLot").val();
    resetAreasDropdownAddParkingLot();
    await $.getJSON(`http://localhost:1028/api/cities/${cityId}/areas`, function (data) {
        for (var i = 0; i < data.length; i++) {
            let element = `<option value="${data[i].id}">${data[i].name}</option>`;
            $("#selectAreaAddParkingLot").append(element);
        }
    })
}

function resetAreasDropdownAddParkingLot() {
    let initialElement = `<option value="" disabled selected hidden>Area</option>`;
    $("#selectAreaAddParkingLot").empty();
    $("#selectAreaAddParkingLot").append(initialElement);
}

$("#newParkingLotSubmit").click(async function (event) {
    event.preventDefault();
    let cityId = $("#selectCityAddParkingLot").val();
    let areaId = $("#selectAreaAddParkingLot").val();
    let name = $("#parkingLotNameAddParkingLot").val();
    let userId = $("#selectUserAddParkingLot").val();
    console.log(userId);
    
    let data = {
        "areaId": areaId,
        "name": name,
        "location": "placeholder for Long/Lat or street",
        "isPaid": true,
        "isStateOwned": false,
        "totalParkingSpaces": 21,
        "hasSecurity": true,
        "applicationUserId": userId
    }

    await $.ajax({
        type: "POST",
        url: `http://localhost:1028/api/cities/${cityId}/areas/${areaId}/parkinglots`,
        data: JSON.stringify(data),
        contentType: "application/json; charset=utf-8",
        crossDomain: true,
        dataType: "json",
        success: function () {
            console.log("Parking lot added successfully.");
        },
        error: function (jqXHR, status) {
            // error handler
            console.log(jqXHR);
            console.log('fail' + status.code);
        }
    })
});

// Initial City Dropdowns populating with options
async function getCitiesFromDb() {
    allCities = [];
    await $.getJSON(`http://localhost:1028/api/cities`, function (data) {
        for (var i = 0; i < data.length; i++) {
            allCities.push(data[i]);
        }   
    });
    await populateCities();
    populateCitiesSelectAddArea();
    populateCitiesSelectAddParkingLot();
    allCities = [];
}

async function populateCities() {
    $("#citiesContainer").empty();
    for (var i = 0; i < allCities.length; i++) {
        $("#citiesContainer").append(
            `<div id="city${allCities[i].id}"><h2>${allCities[i].name}
                    <span><button id="editCity${allCities[i].id}" class="btn btn-warning">Edit</button></span>
                    <span><button id="deleteCity${allCities[i].id}"class="btn btn-danger">Delete</button></span></h2>
            </div>`
        );    
    }
    $("[class*=btn][class*=btn-warning]").click(function () {
        let cityId = this.id.replace("editCity", "");
        handleEditCity(cityId);
    });
    $("[class*=btn][class*=btn-danger]").click(function () {
        let cityId = this.id.replace("deleteCity", "");
        handleDeleteCity(cityId);
    });
}

function populateCitiesSelectAddArea() {
    $("#selectCityAddArea").empty();
    $("#selectCityAddArea").append(`<option value="" disabled selected hidden>City</option>`);
    for (var i = 0; i < allCities.length; i++) {
        let element = `<option value="${allCities[i].id}">${allCities[i].name}</option>`;
        $("#selectCityAddArea").append(element);
    }

}

function populateCitiesSelectAddParkingLot() {
    $("#selectCityAddParkingLot").empty();
    $("#selectCityAddParkingLot").append(`<option value="" disabled selected hidden>City</option>`);
    for (var i = 0; i < allCities.length; i++) {
        let element = `<option value="${allCities[i].id}">${allCities[i].name}</option>`;
        $("#selectCityAddParkingLot").append(element);
    }
}

// Deleting a City
async function handleDeleteCity(cityId) {
    await $.ajax({
        url: `http://localhost:1028/api/cities/${cityId}`,
        type: 'DELETE',
        success: function (result) {
            console.log("City deleted successfully.");
        }
    });
    await $.getJSON(`http://localhost:1028/api/cities`, async function (data) {
        for (var i = 0; i < data.length; i++) {
            allCities.push(data[i]);
        }
        $("#citiesContainer").empty();
        getCitiesFromDb();
    });
};

// Editing a City
async function handleEditCity(cityId) {
    let cityName = "";
    await $.getJSON(`http://localhost:1028/api/cities/${cityId}`, async function (data) {
        cityName = data.name;
    });
    let formElement = `
                        <hr />                                     
                        <div class="form-group">
                            <input type="text" class="form-control" id="editCityName" value="${cityName}">
                        </div>
                        <button id="editCitySubmit${cityId}" class="btn btn-primary">Confirm Edit</button>
                        `
    $("#editCityFormContainer").append(formElement);
    $(`#editCitySubmit${cityId}`).click(async function () {
        let newCityName = $("#editCityName").val();
        await confirmCityEdit(cityId, newCityName);
        await $.getJSON(`http://localhost:1028/api/cities`, async function (data) {
            for (var i = 0; i < data.length; i++) {
                allCities.push(data[i]);
            }
            $("#citiesContainer").empty();
            getCitiesFromDb();
        });
    })
};

async function confirmCityEdit(cityId, newCityName) {
    let data = {
        "id": cityId,
        "name": newCityName
    }
    await $.ajax({
        url: `http://localhost:1028/api/cities/${cityId}`, 
        type: "PUT",        
        data: JSON.stringify(data),
        contentType: "application/json; charset=utf-8",
        crossDomain: true,
        dataType: "json",
        success: function () {
            console.log("edit successful");
        },
        error: function (jqXHR, status) {
            console.log(jqXHR);
            console.log('fail' + status.code);
        }
    })
    $("#editCityFormContainer").empty();    
};

