"use strict";

// enrollmentConnection is used to receive real-time updates about course enrollments and session enrollments.
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

        //update enrolled count
        document
            .querySelectorAll(`[data-enrolled-count-for='${courseId}']`)
            .forEach(element => {
                element.textContent = data.enrolledCount;
            });

        //update enroll buttons
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

    // Instructor and training-coordinator pages
    if (sessionId) {

        //update enrolled count
        document
            .querySelectorAll(`[data-session-enrolled-count-for='${sessionId}']`)
            .forEach(element => {
                element.textContent = data.enrolledCount;
            });

    }
}

// Listen for enrollment updates from the server
enrollmentConnection.on("EnrollmentUpdated", updateEnrollmentDisplay);

async function joinEnrollmentGroups() {
    // Find all course and session elements with data attributes
    const courseElements = document.querySelectorAll("[data-course-id]");
    const sessionElements = document.querySelectorAll("[data-session-id]");

    // Join course groups
    for (const element of courseElements) {
        const courseId = parseInt(element.dataset.courseId);

        if (!isNaN(courseId)) {
            await enrollmentConnection.invoke("JoinCourseGroup", courseId);
            console.log(`Joined course group: ${courseId}`);
        }
    }

    // Join session groups
    for (const element of sessionElements) {
        const sessionId = parseInt(element.dataset.sessionId);

        if (!isNaN(sessionId)) {
            await enrollmentConnection.invoke("JoinSessionGroup", sessionId);
            console.log(`Joined session group: ${sessionId}`);
        }
    }
}


// Start the SignalR connection and join groups on page load
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


startEnrollmentConnection();