function addService() {    
    const container = document.getElementById("services-container");
    const div = document.createElement("div");
    const select = document.createElement("select");    

    div.className = "form-group d-flex gap-2";
    select.className = "form-control";
    select.name = `ServiceSubscriptionIds[${container.childElementCount}]`;
    
    div.appendChild(select);

    allServices.forEach(service => {
        const option = document.createElement("option");
        option.value = service.value;
        option.textContent = service.text;
        select.appendChild(option);
    });

    const button = document.createElement("button");
    button.type = "button";
    button.className = "btn btn-sm btn-secondary mt-2";
    button.onclick = function () {
        removeService(this);
    };
    button.textContent = "Remove Service";
    div.appendChild(button);

    container.appendChild(div);
}

function removeService(button) {
    const container = document.getElementById("services-container");
    const parent = button.parentNode;
    container.removeChild(parent);
}