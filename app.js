const landingPage = document.getElementById('landingPage');
const addCoursePage = document.getElementById('addCoursePage');
const scoreRoundPage = document.getElementById('scoreRoundPage');
const holesContainer = document.getElementById('holesContainer');
const totalScoreEl = document.getElementById('totalScore');
const scoreForm = document.getElementById('scoreForm');

let courses = JSON.parse(localStorage.getItem('courses')) || [
  {
    name: "Pebble Beach",
    yardage: [400, 350, 180, 420, 500, 160, 390, 410, 530, 400, 360, 170, 430, 510, 150, 380, 400, 520],
    par:     [4,   4,   3,   4,   5,   3,   4,   4,   5,   4,   4,   3,   4,   5,   3,   4,   4,   5]
  },
  {
    name: "Augusta National",
    yardage: [400, 350, 180, 420, 500, 160, 390, 410, 530, 400, 360, 170, 430, 510, 150, 380, 400, 520],
    par:     [4,   4,   3,   4,   5,   3,   4,   4,   5,   4,   4,   3,   4,   5,   3,   4,   4,   5]
  }
];



// Routing
function showAddCourse() {
  landingPage.classList.add('hidden');
  addCoursePage.classList.remove('hidden');
}

function showScoreRound() {
  landingPage.classList.add('hidden');
  scoreRoundPage.classList.remove('hidden');
  renderHoles();
}

function goHome() {
  addCoursePage.classList.add('hidden');
  scoreRoundPage.classList.add('hidden');
  landingPage.classList.remove('hidden');
}

// // Save course name
// function saveCourse() {
//   const name = document.getElementById('courseName').value;
//   if (name) {
//     localStorage.setItem('courseName', name);
//     alert(`Course "${name}" saved!`);
//     document.getElementById('courseName').value = '';
//   }
// }

// Render 18 holes
function renderHoles() {
  holesContainer.innerHTML = '';
  for (let i = 1; i <= 18; i++) {
    const div = document.createElement('div');
    div.className = 'hole';

    const label = document.createElement('label');
    label.textContent = `Hole ${i}:`;

    const input = document.createElement('input');
    input.type = 'number';
    input.min = 1;
    input.value = localStorage.getItem(`hole${i}`) || '';
    input.id = `hole${i}`;

    input.addEventListener('input', updateTotal);

    div.appendChild(label);
    div.appendChild(input);
    holesContainer.appendChild(div);
  }
  updateTotal();
}

// Update total score
function updateTotal() {
  let total = 0;
  for (let i = 1; i <= 18; i++) {
    const val = parseInt(document.getElementById(`hole${i}`).value);
    if (!isNaN(val)) total += val;
  }
  totalScoreEl.textContent = total;
}

// Save scores
scoreForm.addEventListener('submit', (e) => {
  e.preventDefault();
  for (let i = 1; i <= 18; i++) {
    const val = document.getElementById(`hole${i}`).value;
    localStorage.setItem(`hole${i}`, val);
  }
  alert('Scores saved!');
});

////////////////////////////////////////////
//////////////////CHOOSE COURSE//////////////
let selectedCourse = null;

function showCourseSelect() {
  landingPage.classList.add('hidden');
  courseSelectPage.classList.remove('hidden');

  const select = document.getElementById('courseDropdown');
  select.innerHTML = '';
  courses.forEach((course, index) => {
    const option = document.createElement('option');
    option.value = index;
    option.textContent = course.name;
    select.appendChild(option);
  });
}

function confirmCourse() {
  const index = document.getElementById('courseDropdown').value;
  selectedCourse = courses[index];
  courseSelectPage.classList.add('hidden');
  scoreRoundPage.classList.remove('hidden');
  renderHolesWithCourse();
}

function renderHolesWithCourse() {
  holesContainer.innerHTML = '';
  for (let i = 0; i < 18; i++) {
    const div = document.createElement('div');
    div.className = 'hole';

    const label = document.createElement('label');
    label.textContent = `Hole ${i + 1} (Par ${selectedCourse.par[i]}, ${selectedCourse.yardage[i]} yds):`;

    const input = document.createElement('input');
    input.type = 'number';
    input.min = 1;
    input.id = `hole${i + 1}`;
    input.value = localStorage.getItem(`hole${i + 1}`) || '';

    input.addEventListener('input', updateTotal);

    div.appendChild(label);
    div.appendChild(input);
    holesContainer.appendChild(div);
  }
  updateTotal();
}

function showAddCourse() {
  landingPage.classList.add('hidden');
  addCoursePage.classList.remove('hidden');

  const container = document.getElementById('courseDetails');
  container.innerHTML = '';

  for (let i = 1; i <= 18; i++) {
    const row = document.createElement('div');
    row.className = 'course-row';

    const label = document.createElement('label');
    label.textContent = `Hole ${i}:`;

    const yardInput = document.createElement('input');
    yardInput.type = 'number';
    yardInput.placeholder = 'Yardage';
    yardInput.id = `yard${i}`;

    const parInput = document.createElement('input');
    parInput.type = 'number';
    parInput.placeholder = 'Par';
    parInput.id = `par${i}`;

    row.appendChild(label);
    row.appendChild(yardInput);
    row.appendChild(parInput);
    container.appendChild(row);
  }
}

function saveCourse() {
  const name = document.getElementById('courseName').value.trim();
  if (!name) return alert("Please enter a course name.");

  const yardage = [];
  const par = [];

  for (let i = 1; i <= 18; i++) {
    const yard = parseInt(document.getElementById(`yard${i}`).value);
    const p = parseInt(document.getElementById(`par${i}`).value);

    if (isNaN(yard) || isNaN(p)) {
      return alert(`Please enter valid yardage and par for hole ${i}.`);
    }

    yardage.push(yard);
    par.push(p);
  }

  const newCourse = { name, yardage, par };
  courses.push(newCourse);
  localStorage.setItem('courses', JSON.stringify(courses));

  alert(`Course "${name}" added!`);
  document.getElementById('courseName').value = '';
  showAddCourse(); // reset form
}



