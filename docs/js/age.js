let bday = new Date(2003, 0, 31); // December 17, 2001 (months are 0-indexed)

let today = new Date();

let age = today.getFullYear() - bday.getFullYear();

// Check if the birthday has occurred this year 
if (today.getMonth() < bday.getMonth() || (today.getMonth() === bday.getMonth() && today.getDate() < bday.getDate())) {
    age--;
}

document.getElementById("dynAge").textContent = `📅 ${age} years old 🌐 United States 🎓 Bachelor of Science in Information Technology`;