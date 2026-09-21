const yardages = [412, 388, 179, 290, 317, 517, 137, 469, 338, 340, 505, 181, 271, 154, 397, 411, 147, 564];
const pars = [4, 4, 3, 4, 4, 5, 3, 5, 4, 4, 5, 3, 4, 3, 4, 4, 3, 5];
const strokeIndexes = [1, 3, 7, 15, 11, 5, 17, 13, 9, 8, 12, 10, 18, 14, 2, 4, 16, 6];
const players = [{ id: 1, name: 'Phil' }, { id: 2, name: 'Brian' }];
let roundId = 0;
let playerIndex = 0;

const byId = id => document.getElementById(id);
const today = new Date().toISOString().slice(0, 10);
byId('round-date').value = today;
byId('leaderboard-year').value = new Date().getFullYear();

function renderScorecard() {
  const rows = [];
  rows.push(`<div class="scorecard-title">Front 9 / Back 9</div>`);
  rows.push(`<div class="scorecard-scroll">${renderNine(0, 'OUT')}${renderNine(9, 'IN')}</div>`);
  byId('scorecard').innerHTML = rows.join('');
  document.querySelector('.score-input').focus();
  document.querySelectorAll('.score-input').forEach(input => input.addEventListener('input', updateScoreSummary));
}

function renderNine(startHole, totalLabel) {
  const holes = Array.from({ length: 9 }, (_, offset) => startHole + offset);
  const parTotal = pars.slice(startHole, startHole + 9).reduce((sum, value) => sum + value, 0);
  return `<div class="score-row heading"><div class="score-cell">HOLE</div>${holes.map(hole => `<div class="score-cell">${hole + 1}</div>`).join('')}<div class="score-cell total-column">${totalLabel}</div></div>` +
    `<div class="score-row"><div class="score-cell label">YARDS</div>${holes.map(hole => `<div class="score-cell">${yardages[hole]}</div>`).join('')}<div class="score-cell total-column">${yardages.slice(startHole, startHole + 9).reduce((sum, value) => sum + value, 0)}</div></div>` +
    `<div class="score-row"><div class="score-cell label">PAR</div>${holes.map(hole => `<div class="score-cell">${pars[hole]}</div>`).join('')}<div class="score-cell total-column">${parTotal}</div></div>` +
    `<div class="score-row"><div class="score-cell label">STROKE INDEX</div>${holes.map(hole => `<div class="score-cell">${strokeIndexes[hole]}</div>`).join('')}<div class="score-cell total-column">-</div></div>` +
    `<div class="score-row"><div class="score-cell label">SCORE</div>${holes.map(hole => `<div class="score-cell"><span class="score-marker"><input class="score-input" data-hole="${hole}" data-par="${pars[hole]}" type="number" min="1" max="20" inputmode="numeric"></span></div>`).join('')}<div class="score-cell total-column live-total" id="${totalLabel.toLowerCase()}-total">0</div></div>`;
}

function updateScoreSummary() {
  const inputs = [...document.querySelectorAll('.score-input')];
  const scores = inputs.map(input => Number(input.value) || 0);
  const total = scores.reduce((sum, score) => sum + score, 0);
  const outTotal = scores.slice(0, 9).reduce((sum, score) => sum + score, 0);
  const inTotal = scores.slice(9).reduce((sum, score) => sum + score, 0);
  byId('score-total').textContent = total;
  byId('out-total').textContent = outTotal;
  byId('in-total').textContent = inTotal;
  inputs.forEach(input => {
    const marker = input.parentElement;
    marker.classList.remove('score-eagle', 'score-birdie', 'score-par', 'score-bogey', 'score-double-bogey');
    const score = Number(input.value);
    if (!score) return;
    const difference = score - Number(input.dataset.par);
    marker.classList.add(difference <= -2 ? 'score-eagle' : difference === -1 ? 'score-birdie' : difference === 0 ? 'score-par' : difference === 1 ? 'score-bogey' : 'score-double-bogey');
  });
  const difference = total - pars.reduce((sum, par) => sum + par, 0);
  byId('score-status').textContent = difference === 0 ? 'Par: E' : difference > 0 ? `Par: +${difference}` : `Par: ${difference}`;
}

byId('start-round').addEventListener('click', async () => {
  const response = await fetch('/api/rounds', { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify({ datePlayed: byId('round-date').value }) });
  if (!response.ok) return alert('Could not start the round.');
  ({ roundId } = await response.json());
  playerIndex = 0;
  byId('round-label').textContent = `Round ${roundId}`;
  byId('start-view').classList.add('hidden');
  byId('leaderboard-view').classList.add('hidden');
  byId('score-view').classList.remove('hidden');
  byId('player-label').textContent = players[playerIndex].name;
  byId('player-step').textContent = 'Player 1 of 2';
  byId('save-scores').textContent = "Save Phil's Scores";
  renderScorecard();
});

byId('back-to-start').addEventListener('click', async () => {
  if (roundId > 0) {
    await fetch(`/api/rounds/${roundId}`, { method: 'DELETE' });
  }
  roundId = 0;
  playerIndex = 0;
  byId('score-view').classList.add('hidden');
  byId('start-view').classList.remove('hidden');
  byId('leaderboard-view').classList.remove('hidden');
  loadLeaderboard();
});

byId('save-scores').addEventListener('click', async () => {
  const inputs = [...document.querySelectorAll('.score-input')];
  const scores = inputs.map(input => Number(input.value));
  if (scores.some(score => !Number.isInteger(score) || score < 1 || score > 20)) return alert('Enter a score for every hole.');
  const response = await fetch(`/api/rounds/${roundId}/scores`, { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify({ playerId: players[playerIndex].id, scores }) });
  if (!response.ok) return alert('Could not save scores.');
  if (playerIndex === 0) {
    playerIndex = 1;
    byId('player-label').textContent = players[playerIndex].name;
    byId('player-step').textContent = 'Player 2 of 2';
    byId('save-scores').textContent = "Save Brian's Scores";
    renderScorecard();
    return;
  }
  alert(`Round ${roundId} saved.`);
  byId('score-view').classList.add('hidden');
  byId('start-view').classList.remove('hidden');
  byId('leaderboard-view').classList.remove('hidden');
  loadLeaderboard();
});

async function loadLeaderboard() {
  const year = byId('leaderboard-year').value;
  const response = await fetch(`/api/leaderboard?year=${year}`);
  const rounds = await response.json();
  const totals = {};
  rounds.forEach(round => {
    totals[round.player1] = (totals[round.player1] || 0) + round.player1Score;
    totals[round.player2] = (totals[round.player2] || 0) + round.player2Score;
  });
  const standings = Object.entries(totals).sort((a, b) => a[1] - b[1]);
  byId('annual-tracker').textContent = standings.length < 2 ? 'Waiting for both players' : standings[0][1] === standings[1][1] ? 'TIE' : `${standings[0][0]} UP ${standings[1][1] - standings[0][1]}`;
  byId('leaderboard-list').innerHTML = rounds.length ? rounds.map(round => `<article class="round-entry"><div class="round-top">Round ${round.roundId} | ${round.datePlayed}</div><div class="round-scores">${round.player1}: ${round.player1Score} &nbsp; ${round.player2}: ${round.player2Score}</div><div class="round-result">${round.result}</div></article>`).join('') : '<p class="muted">No rounds found for this year.</p>';
}

byId('load-year').addEventListener('click', loadLeaderboard);
byId('leaderboard-year').addEventListener('change', loadLeaderboard);
loadLeaderboard();
