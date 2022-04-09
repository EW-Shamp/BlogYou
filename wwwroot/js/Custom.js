
let index = 0;
function AddTag() {
    //Get a reference to the TagEntry input element
    var tagEntry = document.getElementById("TagEntry");

    //Uses search function to detect error state
    let searchResult = search(tagEntry.value);
    if (searchResult != null) {
        //Trigger sweet alert for the error condition that is contained in searchResult
        swalWithDarkButton.fire({
            html: `<span class='font-weight-bolder'>${searchResult.toUpperCase()}</span?`,
        });
    } else {
        //Creat a new Select Option
        let newOption = new Option(tagEntry.value, tagEntry.value);
        document.getElementById("TagList").options[index++] = newOption;
    }

    //Clear out the TagEntry control
    tagEntry.value = "";
    return true;
}

function DeleteTag() {
    let tagCount = 1;
    let tagList = document.getElementById("TagList");
    if (!tagList) return false;

    if (tagList.selectedIndex == -1) {
        swalWithDarkButton.fire({
            html: `<span class="font-weight-bolder">CHOOSE TAG BEFORE DELETING</span>`
        });
        return true;
    }

    while (tagCount > 0) {
        if (tagList.selectedIndex >= 0) {
            tagList.options[tagList.selectedIndex] = null;
            --tagCount;
        }
        else
            tagCount = 0;
        index--;
    }
}

$("form").on("submit", function () {
    $("#TagList option").prop("selected", "selected");
})

//Look for the tagValues variable to see if it has data
if (tagValues != '') {
    let tagArray = tagValues.split(",");
    for (let loop = 0; loop < tagArray.length; loop++) {
        //load up or replace the options we have
        ReplaceTag(tagArray[loop], loop);
        index++;
    }
}

function ReplaceTag(tag, index) {
    let newOption = new Option(tag, tag);
    document.getElementById("TagList").options[index] = newOption;
}

//The Search function will detect eiither an empthy or a duplicate Tag
//and return an error string if an error is detected
function search(str) {
    if (str == "") {
        return "Empty tags are not permitted";
    }

    var tagsEl = document.getElementById('TagList');
    if (tagsEl) {
        let options = tagsEl.options;
        for (let i = 0; i < options.length; i++) {
            if (options[i].value = str) {
                return `The tag #${str} was detected as a duplicate and not permitted`;
            }
        }
    }
}

const swalWithDarkButton = Swal.mixin({
    customClass: {
        confirmButton: 'btn btn-danger btn-sm note-btn-block btn-outline-dark'
    },
    imageUrl: '/img/oops.jpg',
    timer: 3000,
    buttonsStyling: false
});