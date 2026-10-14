const tg = window.Telegram && window.Telegram.WebApp ? window.Telegram.WebApp : null;

if (tg) {
    tg.ready();
    tg.expand(); // Разворачиваем в Telegram на весь экран
}
const canvas = document.getElementById("gameCanvas");
const ctx = canvas.getContext("2d"); 

let birdStartY = 150;

let bird = { x: 50, y: 150, w: 20, h: 20, gravity: 0.08, lift: -3.5, velocity: 0 };
let pipes = [];
let pipeWidth = 50;
let pipeGap = 160;
let pipeSpeed = 2;
let score = 0; 
let bestScore = 0; // Максимальный рекорд текущей сессии
let isGameOver = false;
let frameCount = 0;

// Размеры и координаты интерактивной кнопки "Submit" на холсте
const submitBtn = { x: 60, y: 340, w: 200, h: 45 };

window.addEventListener("keydown", (e) => { 
    if (e.code === "Space") handleAction(null); 
});
canvas.addEventListener("touchstart", (e) => { 
    e.preventDefault(); 
    const rect = canvas.getBoundingClientRect();
    const touch = e.touches[0];
    handleAction({ x: touch.clientX - rect.left, y: touch.clientY - rect.top }); 
});
canvas.addEventListener("mousedown", (e) => { 
    const rect = canvas.getBoundingClientRect();
    handleAction({ x: e.clientX - rect.left, y: e.clientY - rect.top }); 
});

// Единый обработчик нажатий
function handleAction(clickCoords) {
    if (isGameOver) {
        // Если клик зафиксирован и он попал в границы кнопки "Submit Record"
        if (clickCoords && 
            clickCoords.x >= submitBtn.x && clickCoords.x <= submitBtn.x + submitBtn.w &&
            clickCoords.y >= submitBtn.y && clickCoords.y <= submitBtn.y + submitBtn.h) {
            sendResultToBot();
            return;
        }
        // Клик в любое другое место экрана просто перезапускает раунд
        resetGame();
        return;
    }
    
    // Обычный полет птички
    bird.velocity = bird.lift;
}

function sendResultToBot() {
    const dataToSend = {
        score: bestScore
    };

    // Проверяем наличие самого объекта tg и метода sendData
    if (tg && typeof tg.sendData === 'function') {
        try {
            tg.sendData(JSON.stringify(dataToSend));
            
            // Даем Telegram 150мс на отправку строки, затем закрываем
            setTimeout(() => {
                tg.close();
            }, 150);
        } catch (error) {
            alert("Ошибка отправки через Telegram: " + error.message);
        }
    } else {
        // Этот блок сработает только на реальном ПК в браузере
        alert(`[PC Test] Sent best score: ${bestScore} to bot!\nGame window closed.`);
        bestScore = 0;
        resetGame();
    }
}

function spawnPipe() {
    let minHeight = 40;
    let maxHeight = canvas.height - pipeGap - minHeight;
    let height = Math.floor(Math.random() * (maxHeight - minHeight + 1)) + minHeight;

    pipes.push({
        x: canvas.width,
        top: height,
        bottom: canvas.height - height - pipeGap,
        passed: false
    });
}

function resetGame() {
    bird.y = birdStartY;
    bird.velocity = 0;
    pipes = [];
    score = 0;
    isGameOver = false;
    frameCount = 0;
}

function update() {
    if (isGameOver) return;

    frameCount++;
    bird.velocity += bird.gravity;

    if (bird.velocity > 8) {
        bird.velocity = 8;
    }

    bird.y += bird.velocity;

    if (bird.y + bird.h >= canvas.height || bird.y <= 0) {
        isGameOver = true;
        checkBestScore();
    }

    if (frameCount % 100 === 0) spawnPipe();

    for (let i = pipes.length - 1; i >= 0; i--) {
        pipes[i].x -= pipeSpeed;

        if (bird.x < pipes[i].x + pipeWidth &&
            bird.x + bird.w > pipes[i].x &&
            (bird.y < pipes[i].top || bird.y + bird.h > canvas.height - pipes[i].bottom)) {
            isGameOver = true;
            checkBestScore();
        }
        
        if (!pipes[i].passed && pipes[i].x + pipeWidth < bird.x) {
            score++;
            pipes[i].passed = true;
        }

        if (pipes[i].x + pipeWidth < 0) pipes.splice(i, 1);
    }
}

// Проверка и обновление лучшего результата
function checkBestScore() {
    if (score > bestScore) {
        bestScore = score;
    }
}

function draw() {
    ctx.clearRect(0, 0, canvas.width, canvas.height);

    // Рисуем птичку
    ctx.fillStyle = "#ffeb3b";
    ctx.fillRect(bird.x, bird.y, bird.w, bird.h);

    // Рисуем трубы
    ctx.fillStyle = "#4caf50";
    for (let pipe of pipes) {
        ctx.fillRect(pipe.x, 0, pipeWidth, pipe.top);
        ctx.fillRect(pipe.x, canvas.height - pipe.bottom, pipeWidth, pipe.bottom);
    }

    // Рисуем текущий счет вверху экрана во время игры
    ctx.fillStyle = "#ffffff";
    ctx.font = "20px Arial";
    ctx.textAlign = "left";
    ctx.fillText(`Score: ${score}`, 15, 35);
    ctx.fillText(`Best: ${bestScore}`, 15, 60);

    // Экран Game Over
    if (isGameOver) {
        ctx.fillStyle = "rgba(0, 0, 0, 0.75)";
        ctx.fillRect(0, 0, canvas.width, canvas.height);

        ctx.fillStyle = "#ff3d00";
        ctx.font = "bold 34px Arial";
        ctx.textAlign = "center";
        ctx.fillText("Game Over", canvas.width / 2, canvas.height / 2 - 90);

        // Статистика
        ctx.fillStyle = "#ffffff";
        ctx.font = "20px Arial";
        ctx.fillText(`Current Score: ${score}`, canvas.width / 2, canvas.height / 2 - 30);
        
        ctx.fillStyle = "#ffeb3b";
        ctx.font = "bold 22px Arial";
        ctx.fillText(`Session Best: ${bestScore}`, canvas.width / 2, canvas.height / 2 + 10);

        ctx.fillStyle = "#b0bec5";
        ctx.font = "15px Arial";
        ctx.fillText("Tap anywhere to TRY AGAIN", canvas.width / 2, canvas.height / 2 + 50);

        // Рисуем интерактивную кнопку отправки рекорда
        ctx.fillStyle = "#4caf50";
        ctx.beginPath();
        ctx.roundRect ? ctx.roundRect(submitBtn.x, submitBtn.y, submitBtn.w, submitBtn.h, 10) : ctx.rect(submitBtn.x, submitBtn.y, submitBtn.w, submitBtn.h);
        ctx.fill();

        // Текст внутри кнопки
        ctx.fillStyle = "#ffffff";
        ctx.font = "bold 16px Arial";
        ctx.fillText("🚀 SUBMIT RECORD & EXIT", canvas.width / 2, submitBtn.y + 28);
    }
}

function gameLoop() {
    update();
    draw();
    requestAnimationFrame(gameLoop);
}

gameLoop();
