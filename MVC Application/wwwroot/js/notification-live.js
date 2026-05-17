"use strict";

// notificationConnection is used to receive real-time notifications for the user.
const notificationConnection = new signalR.HubConnectionBuilder()
    .withUrl("/hubs/notifications")
    .withAutomaticReconnect()
    .build();

// Listen for incoming notifications from the server
notificationConnection.on("ReceiveNotification", function (notification) {
    console.log("Notification received:", notification);

    showNotificationToast(notification);
    increaseNotificationBadge();
});

// Displays a toast notification on the screen with the given notification data
function showNotificationToast(notification) {

    // Create the container for toasts 
    let container = document.createElement("div");

    // Give the container an ID and style it to appear in the top-right corner of the screen
        container.id = "notification-toast-container";
        container.style.position = "fixed";
        container.style.top = "20px";
        container.style.right = "20px";
        container.style.zIndex = "9999";
        document.body.appendChild(container);
    
    // Create the toast element with Bootstrap classes 
    const toast = document.createElement("div");
    toast.className = "alert alert-info shadow-sm d-flex justify-content-between align-items-start gap-3";
    toast.style.minWidth = "320px";
    toast.style.marginBottom = "10px";

    // Set the content of the toast using the notification data
    toast.innerHTML = `
        <div>
            <strong>New Notification</strong>
            <div>${notification.message}</div>
        </div>

        <button type="button" class="btn-close" aria-label="Close"></button>
    `;

    // Add event listener to the close button to mark the notification as read and remove the toast
    const closeButton = toast.querySelector(".btn-close");

    closeButton.addEventListener("click", async function () {
        const success = await markNotificationAsRead(notification.id);

        if (success) {
            // If the server successfully marked the notification as read, decrease the badge count and remove the toast
            decreaseNotificationBadge();
            toast.remove();
        }
    });

    container.appendChild(toast);
}

// Increases the notification badge count by 1 and makes it visible if it was previously hidden
function increaseNotificationBadge() {
    // Get the badge element that displays the notification count
    const badge = document.getElementById("notification-count");

    // If the badge element doesn't exist on the page, we can't update it, so we return early
    if (!badge) {
        return;
    }

    // Parse the current count from the badge's text content, defaulting to 0 if it's empty or invalid
    const currentCount = parseInt(badge.textContent || "0");
    // Increment the count by 1
    const newCount = currentCount + 1;

    // Update the badge's text content with the new count and ensure it's visible
    badge.textContent = newCount;
    // Remove the "d-none" class to make the badge visible if it was previously hidden
    // (The "d-none" is added when decreaseNotificationBadge() is used and the notification count becomes 0)
    badge.classList.remove("d-none");
}

function decreaseNotificationBadge() {
    // Get the badge element that displays the notification count
    const badge = document.getElementById("notification-count");

    // If the badge element doesn't exist on the page, we can't update it, so we return early
    if (!badge) {
        return;
    }

    // Parse the current count from the badge's text content, defaulting to 0 if it's empty or invalid
    const currentCount = parseInt(badge.textContent || "0");
    // Decrement the count by 1, ensuring it doesn't go below 0
    const newCount = Math.max(currentCount - 1, 0);

    // Update the badge's text content with the new count
    badge.textContent = newCount;

    // If the new count is 0, hide the badge by adding the "d-none" class
    if (newCount === 0) {
        badge.classList.add("d-none");
    }
}

// Sends a request to the server to mark the specified notification as read
async function markNotificationAsRead(notificationId) {
    try {
        // Get the anti-forgery token from the page to include in the request headers
        const tokenInput = document.querySelector("input[name='__RequestVerificationToken']");
        const token = tokenInput ? tokenInput.value : "";

        // Send a POST request to the server to mark the notification as read
        const response = await fetch("/Notifications/MarkAsRead", {
            method: "POST",
            headers: {
                "Content-Type": "application/x-www-form-urlencoded",
                "RequestVerificationToken": token
            },
            body: `id=${encodeURIComponent(notificationId)}`
        });

        return response.ok;
    } catch (error) {
        console.error("Failed to mark notification as read:", error);
        return false;
    }
}

// Start the SignalR connection to receive notifications from the server
async function startNotificationConnection() {
    try {
        await notificationConnection.start();
        console.log("Connected to NotificationHub");
    } catch (err) {
        console.error("Notification SignalR connection failed:", err);
        setTimeout(startNotificationConnection, 3000);
    }
}


startNotificationConnection();