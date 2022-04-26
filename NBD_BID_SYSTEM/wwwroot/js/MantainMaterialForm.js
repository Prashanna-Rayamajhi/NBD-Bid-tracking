//getting the dom element for bid material list
let materialListDiv = document.getElementById("materialLists");
const selectListInv = document.getElementById("selectInvList");
const listgroupDiv = document.getElementById("listGroup");
const materialFormDiv = document.getElementById("materialForm");
const addedMaterialListRow = document.getElementById("materialData");
const addedmaterialListDiv = document.getElementById("addedMaterialList");


//loading the data in the screen
$.getJSON("/Bids/GetInventoryType", function (response) {
    response.forEach(data => {
        let option = document.createElement('option');
        option.setAttribute("value", data.id.toString());
        //let text = document.createTextNode(`${data.descOfType}`);
        option.textContent = data.descOfType
        selectListInv.appendChild(option);
    })
});

//loading the data in list group 
function loadInventoryItems() {
    $.getJSON("/Bids/GetInventoryItems?", function (response) {
       
        response.forEach(data => {
            listgroupDiv.innerHTML += `<button type="button" onclick="btnClicked(${data.id})" class="list-group-item list-group-item-action">
<input type="hidden" InventoryID=${data.id} />
 <div class="d-flex w-100 justify-content-between">
      <h5 class="mb-1">${data.name} Code: ${data.code}</h5>
    </div>
    <p class="mb-1">${data.size}</p>
    <small>$ ${data.price}</small>
</button><br />`
        });

   
    });
}
loadInventoryItems();

function loadInventoryItemsByType(typeID) {
    $.getJSON(`/Bids/GetInventoryItems?typeID=${typeID.toString()}`, function (response) {

        response.forEach(data => {
            listgroupDiv.innerHTML += `<button type="button" onclick="btnClicked(${data.id})" class="list-group-item list-group-item-action" >
 <div class="d-flex w-100 justify-content-between">
      <h5 class="mb-1">${data.name} Code: ${data.code}</h5>
    </div>
    <p class="mb-1">${data.size}</p>
    <small>$ ${data.price}</small>
</button><br />`
        })
      
    });
}

//looking for selection change in drop down list
$("#selectInvList").on('change', function (e) {
    listgroupDiv.innerHTML = " ";
    if (this.value == 0) {
        loadInventoryItems();
    } else {
        loadInventoryItemsByType(this.value)
    }
})
function btnClicked(InventoryID) {
    materialFormDiv.innerHTML=""
    materialFormDiv.style.visibility = "visible";
    $.getJSON(`/Bids/GetItemByID?ID=${InventoryID.toString()}`, function (response) {
        materialFormDiv.innerHTML = `<h4>Item Name: ${response[0].name}</h4>
       
    <div class="form-group">
                        <label class="control-label">Quantiy</label>
                        <input type="text" id="materialQty" name="materialQty"  class="form-control w-100" />
                        <span asp-validation-for="Quantity" class="text-danger"></span>
                    </div>
                <div style="overflow:auto;">
                <div style="float:right;">
                    <button type="button" id="addBtn" invID="${response[0].id}" onclick="addData(event)" class="btn btn-outline-primary">Add</button>
                    <button type="button" id="cancelBtn" onclick="removeForm(event)" class="btn btn-outline-dark">Cancel</button>
                </div>

            </div>`
    })

}
let countedMaterial = 0;
//for edit page of bidMaterial section

if (document.title.includes("Edit")) {
    let bidID = window.location.href.slice(-1);
    updateBidMaterialData(bidID);
    document.getElementById("countMaterial").innerHTML == countedMaterial.toString();
}
function updateBidMaterialData(bidID) {
    $.getJSON("/Bids/GetBidMaterialByID?bidID=" + bidID.toString(), function (response) {
        countedMaterial += response.length;
        response.forEach(material => {
            addedMaterialListRow.innerHTML += `<tr id="tr-${material.inventory.id.toString()}"><td>${material.inventory.name}</td>
<td><input type="hidden" name="InventoryID" value="${material.inventory.id.toString()}" />${material.inventory.code}</td>
<td>${material.inventory.price}</td>
<td><input type="hidden" name="Quantity" value="${material.quantity.toString()}" />${material.quantity}</td>
<td><input type="hidden" name="Price" value="${material.price.toString()}" />${material.price}</td>
<td><button type="button" class="btn btn-link" invID="${material.inventory.id}" onclick="editMaterial(event)" qty="${material.quantity}">Edit</button>|<button type="button" class="btn btn-link" onclick="removeTableData('${material.inventory.id}')">Delete</button></td></tr>`;

        })
    })
}


function addData(e) {
    let inventoryID = e.target.getAttribute("invID");
    let countMaterial = document.getElementById("countMaterial");
    let materialQty = parseInt($("#materialQty").val());
    $.getJSON(`/Bids/GetItemByID?ID=${inventoryID.toString()}`, function (response) {

        let cost = (materialQty * response[0].price).toFixed(2)
        addedMaterialListRow.innerHTML += `<tr id="tr-${inventoryID.toString()}"><td>${response[0].name}</td>
<td><input   type="hidden" name="InventoryID" value="${inventoryID.toString()}" />${response[0].code}</td>
<td>${response[0].price}</td>
<td><input   type="hidden" name="Quantity" value="${materialQty.toString()}" />${materialQty}</td>
<td><input type="hidden" name="Price" value="${cost.toString()}" />${cost}</td>
<td><button type="button" class="btn btn-link" invID="${inventoryID}" onclick="editMaterial(event)" qty="${materialQty}">Edit</button>|<button type="button" class="btn btn-link" onclick="removeTableData(${inventoryID})">Delete</button></td></tr>`;
        countedMaterial++;
        countMaterial.innerHTML = countedMaterial.toString();
        materialFormDiv.innerHTML = `<h5 class="mt-3">Successfully Added ${response[0].name} on list</h5>`;
            
    });

}

function removeForm(e) {
    materialFormDiv.innerHTML = "";
}

function removeTableData(inventoryID) {
    
    document.getElementById(`tr-${inventoryID}`).remove();
    countedMaterial--;
    countMaterial.innerHTML = countedMaterial.toString();
}

function editMaterial(e) {

    let inventoryID = e.target.getAttribute("invID");
    let qty = e.target.getAttribute("qty");
    $.getJSON(`/Bids/GetItemByID?ID=${inventoryID.toString()}`, function (response) {
        console.log(response)
        materialFormDiv.innerHTML = `<h4>Item Name: ${response[0].name}</h4>
       
    <div class="form-group">
                        <label class="control-label">Quantiy</label>
                        <input type="text" id="materialQty" name="materialQty" value="${qty}" class="form-control w-100" />
                        <span asp-validation-for="Quantity" class="text-danger"></span>
                    </div>
                <div style="overflow:auto;">
                <div style="float:right;">
                    <button type="button" id="addBtn" invID="${response[0].id}" onclick="editData(event)" class="btn btn-outline-primary">Save</button>
                    <button type="button" id="cancelBtn" onclick="removeForm(event)" class="btn btn-outline-dark">Cancel</button>
                </div>

            </div>`

    });

}

function editData(e) {
    let invID = e.target.getAttribute("invID")

    removeTableData(invID);
    addData(e);
}

