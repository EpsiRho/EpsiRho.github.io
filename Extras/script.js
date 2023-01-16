import { allComponents, baseLayerLuminance, StandardLuminance, provideFluentDesignSystem, }
from 'https://unpkg.com/@fluentui/web-components';
/**
 * Register with the Fluent Design System.
 */
export const FluentDesignSystem = provideFluentDesignSystem().register(allComponents);
baseLayerLuminance.setValueFor(document.body, StandardLuminance.DarkMode);

function sendDateTime() {
    const date = new Date(document.getElementById("datepicker").value + "T" + document.getElementById("timepicker").value);
    const currentTime = date.toUTCString();
    //const socket = new WebSocket('wss://71.146.161.50:12222');
    fetch("http://71.146.161.50:12222", {
            method: "POST",
            body: currentTime,
            //headers: { "Content-Type": "application/json" }
        })
        .then(response => response.json())
        .then(data => {
            console.log("Server response:", data);
        });
}

document.getElementById("submit").addEventListener("click", sendDateTime);