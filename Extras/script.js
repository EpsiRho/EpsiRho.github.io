import { allComponents, baseLayerLuminance, StandardLuminance, provideFluentDesignSystem, }
from 'https://unpkg.com/@fluentui/web-components';
/**
 * Register with the Fluent Design System.
 */
export const FluentDesignSystem = provideFluentDesignSystem().register(allComponents);
baseLayerLuminance.setValueFor(document.body, StandardLuminance.DarkMode);


// JavaScript function to send the date and time
function sendDateTime() {
    const date = new Date(document.getElementById("datepicker").value + "T" + document.getElementById("timepicker").value);
    const currentTime = date.toUTCString();
    const net = require('net');

    const client = new net.Socket();
    client.connect(12222, '71.146.161.50', function() {
        const date = new Date();
        const currentTime = date.toUTCString();
        client.write(currentTime);
    });
}
// add event listener to the submit button
document.getElementById("submit").addEventListener("click", sendDateTime);