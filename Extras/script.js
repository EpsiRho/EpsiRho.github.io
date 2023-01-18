import { allComponents, baseLayerLuminance, StandardLuminance, provideFluentDesignSystem, }
from 'https://unpkg.com/@fluentui/web-components';
/**
 * Register with the Fluent Design System.
 */
export const FluentDesignSystem = provideFluentDesignSystem().register(allComponents);
baseLayerLuminance.setValueFor(document.body, StandardLuminance.DarkMode);

function sendDateTime() {
    const date = new Date(document.getElementById("datepicker").value + "T" + document.getElementById("timepicker").value);
    const currentTime = date.toLocaleString();
    console.log(currentTime);
    fetch("http://71.146.161.50:5238/api/EntryLogger", {
            method: "POST",
            body: currentTime,
            headers: { "Content-Type": "text/plain" }
        })
        .then(data => {
            console.log("Server response:", data);
        });
}

document.getElementById("submit").addEventListener("click", sendDateTime);