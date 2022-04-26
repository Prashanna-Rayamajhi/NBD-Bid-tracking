

let currentTab = 0;
let slideTabs = document.querySelectorAll('.slideTab');
let prevBtn = document.getElementById("prevBtn");
let nextBtn = document.getElementById("nextBtn");
//prevBtn.addEventListener("click", nextPrev(-1));
// nextBtn.addEventListener("click", nextPrev(1));
showTab(currentTab);

function showTab(n) {
    slideTabs[n].style.display = "block";

    if (n == 0) {
        prevBtn.style.display = "none";
    }
    else {
        prevBtn.style.display = "inline";
    }
    if (n == (slideTabs.length - 1)) {
        nextBtn.innerHTML = "Submit";
    }
    else {
        nextBtn.innerHTML = "Next";
    }
    maintainIndicator(n);
}
function maintainIndicator(n) {
    let steps = document.querySelectorAll(".step")
    for (let i = 0; i < steps.length; i++) {
        steps[i].className = steps[i].className.replace(" active", "");
    }
    //... and adds the "active" class to the current step:
    steps[n].className += " active";
    
}
function nextPrev(n, bidId) {
    // This function will figure out which tab to display
    //we are in bid labor tab and we need to make dom ready for bidlabor data
    if (currentTab == 0) {
        updateBidLaborForm(bidId);
    };

    // Hide the current tab:
    slideTabs[currentTab].style.display = "none";
    // Increase or decrease the current tab by 1:
    currentTab = currentTab + n;
    // if you have reached the end of the form... :
    if (currentTab >= slideTabs.length) {
        //...the form gets submitted:
        
        document.getElementById("bidForm").submit()
              
        return false;
    }
    // Otherwise, display the correct tab:
    showTab(currentTab);
}

//updating the bidlabor form
function updateBidLaborForm(bidID) {
    document.querySelector(".bidLaborForm").innerHTML = "";
    let bidLaborChkBoxes = document.querySelectorAll("input[name='selectedOptions']");
    let selectedBidLaborID = [];
    let URL = "/Bids/GetLabors?bidID=" + bidID;
   

    bidLaborChkBoxes.forEach(b => {
        if (b.checked) {
            selectedBidLaborID.push(parseInt(b.value));
        }
    });

    $.getJSON(URL, function (data) {
        let dataArr;
        if (bidID == 0) {
            dataArr = data;
          
        } else {
            dataArr = data.result;

        }
       
        if (selectedBidLaborID.length == 0) {
            document.querySelector(".bidLaborForm").innerHTML = "<h6>Please Select bid labor from previous from</h6>";
        }

        dataArr.forEach(d => {
            selectedBidLaborID.forEach(i => {
                if (i == d.id) {
                    if (bidID != 0) {
                        document.querySelector(".bidLaborForm").innerHTML += `<div class="form-group">
                        <label class="control-label">${d.type} Working Hours</label>
                        <input type="text" name="hoursWorked"  value="${d.bidLabors[0].hoursWorked}" class="form-control w-100" />
                        <span asp-validation-for="HoursWorked" class="text-danger"></span>
                    </div>` ;
                    }
                    else {
                        document.querySelector(".bidLaborForm").innerHTML += `<div class="form-group">
                        <label class="control-label">${d.type} Working Hours</label>
                        <input type="text" name="hoursWorked"  value="" class="form-control w-100" />
                        <span asp-validation-for="HoursWorked" class="text-danger"></span>
                    </div>` ;
                    }
                    
                }
            })
        });

    });
   
    
}


