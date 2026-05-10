"use strict";

const enrollmentConnection = new signalR.HubConnectionBuilder()
    .withUrl("/hubs/enrollment")
    .withAutomaticReconnect()
    .build();

function updateEnrollmentDisplay(data) {
    console.log("Enrollment update received:", data);

    const courseId = data.courseId;
    const sessionId = data.sessionId;

    // Trainee course pages
    if (courseId) {
        document
            .querySelectorAll(`[data-remaining-seats-for='${courseId}']`)
            .forEach(element => {
                element.textContent = data.remainingSeats;
            });

        document
            .querySelectorAll(`[data-enrolled-count-for='${courseId}']`)
            .forEach(element => {
                element.textContent = data.enrolledCount;
            });

        document
            .querySelectorAll(`[data-enroll-button-for='${courseId}']`)
            .forEach(button => {
                if (data.isFull) {
                    button.disabled = true;
                    button.classList.add("disabled");
                    button.textContent = "Full";
                }
            });
    }

    // Instructor pages
    if (sessionId) {
        document
            .querySelectorAll(`[data-session-enrolled-count-for='${sessionId}']`)
            .forEach(element => {
                element.textContent = data.enrolledCount;
            });

        document
            .querySelectorAll(`[data-session-capacity-for='${sessionId}']`)
            .forEach(element => {
                element.textContent = data.capacity;
            });

        document
            .querySelectorAll(`[data-session-enrollment-text-for='${sessionId}']`)
            .forEach(element => {
                element.textContent = `${data.enrolledCount} / ${data.capacity}`;
            });

        document
            .querySelectorAll(`[data-session-trainee-count-for='${sessionId}']`)
            .forEach(element => {
                element.textContent = data.enrolledCount;
            });

        document
            .querySelectorAll(`[data-session-status-for='${sessionId}']`)
            .forEach(element => {
                if (data.isFull) {
                    element.textContent = "Full";
                    element.classList.remove("bg-secondary", "bg-success");
                    element.classList.add("bg-danger");
                }
            });
    }
}

enrollmentConnection.on("EnrollmentUpdated", updateEnrollmentDisplay);

async function joinEnrollmentGroups() {
    const courseElements = document.querySelectorAll("[data-course-id]");
    const sessionElements = document.querySelectorAll("[data-session-id]");

    for (const element of courseElements) {
        const courseId = parseInt(element.dataset.courseId);

        if (!isNaN(courseId)) {
            await enrollmentConnection.invoke("JoinCourseGroup", courseId);
            console.log(`Joined course group: ${courseId}`);
        }
    }

    for (const element of sessionElements) {
        const sessionId = parseInt(element.dataset.sessionId);

        if (!isNaN(sessionId)) {
            await enrollmentConnection.invoke("JoinSessionGroup", sessionId);
            console.log(`Joined session group: ${sessionId}`);
        }
    }
}

async function startEnrollmentConnection() {
    try {
        await enrollmentConnection.start();
        console.log("Connected to EnrollmentHub");

        await joinEnrollmentGroups();
    } catch (err) {
        console.error("SignalR connection failed:", err);
        setTimeout(startEnrollmentConnection, 3000);
    }
}

enrollmentConnection.onreconnected(async () => {
    await joinEnrollmentGroups();
});

startEnrollmentConnection();