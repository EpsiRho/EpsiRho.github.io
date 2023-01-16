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
    const socket = new WebSocket('ws://71.146.161.50:12222');

    socket.onopen = function() {
        socket.send(currentTime);
    };
}

document.getElementById("submit").addEventListener("click", sendDateTime);