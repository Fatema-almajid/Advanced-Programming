"use strict";

const notificationConnection = new signalR.HubConnectionBuilder()
    .withUrl("/hubs/notifications")
    .withAutomaticReconnect()
    .build();

notificationConnection.on("ReceiveNotification", function (notification) {
    console.log("Notification received:", notification);

    showNotificationToast(notification);
    increaseNotificationBadge();
});

function showNotificationToast(notification) {
    let container = document.getElementById("notification-toast-container");

    if (!container) {
        container = document.createElement("div");
        container.id = "notification-toast-container";
        container.style.position = "fixed";
        container.style.top = "20px";
        container.style.right = "20px";
        container.style.zIndex = "9999";
        document.body.appendChild(container);
    }

    const toast = document.createElement("div");
    toast.className = "alert alert-info shadow-sm d-flex justify-content-between align-items-start gap-3";
    toast.style.minWidth = "320px";
    toast.style.marginBottom = "10px";

    toast.innerHTML = `
        <div>
            <strong>New Notification</strong>
            <div>${notification.message}</div>
        </div>

        <button type="button" class="btn-close" aria-label="Close"></button>
    `;

    const closeButton = toast.querySelector(".btn-close");

    closeButton.addEventListener("click", async function () {
        const success = await markNotificationAsRead(notification.id);

        if (success) {
            decreaseNotificationBadge();
            toast.remove();
        }
    });

    container.appendChild(toast);
}

function increaseNotificationBadge() {
    const badge = document.getElementById("notification-count");

    if (!badge) {
        return;
    }

    const currentCount = parseInt(badge.textContent || "0");
    const newCount = currentCount + 1;

    badge.textContent = newCount;
    badge.classList.remove("d-none");
}

function decreaseNotificationBadge() {
    const badge = document.getElementById("notification-count");

    if (!badge) {
        return;
    }

    const currentCount = parseInt(badge.textContent || "0");
    const newCount = Math.max(currentCount - 1, 0);

    badge.textContent = newCount;

    if (newCount === 0) {
        badge.classList.add("d-none");
    }
}

async function markNotificationAsRead(notificationId) {
    try {
        const tokenInput = document.querySelector("input[name='__RequestVerificationToken']");
        const token = tokenInput ? tokenInput.value : "";

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