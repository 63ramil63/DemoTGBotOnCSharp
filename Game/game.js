const tg = window.Telegram.WebApp;
tg.ready();
tg.expand();

const canvas = document.getElementById("gameCanvas");
const ctx = canvas.GetContext("2d");

let bird = { x: 50, y: 150, w: 20, h: 20, gravity: 0.6, lift: -10, velocity: 0};
let pipes = [];
let pipeWidth = 50;
let pipeGap = 120;
let pipeSpeed = 2;
let score = 2;
let isGameOver = false;
let frameCount = 0;

window.addEventListener("keydown", (e) => {if (e.code === "Space") jump(); });
canvas.addEventListener("touchstart", (e) => {e.preventDefault(); jump(); });
canvas.addEventListener("mousedown", jump);

function jump() {
    if (isGameOver) {
        sendResultToBot();
        return;
    }
    bird.velocity = bird.lift;
}

function sendResultToBot(){
    const dataToSend = {
        score: score,
        game: "flappy_bird"
    };

    tg.sendData(JSON.stringify(dataToSend));
    tg.close(); // Закрывает Web App окно
}

function spawnPipe() {
    let minHeight = 40;
    let maxHeight = canvas.height - height - pipeGap;
    let height = Math.floor(Math.random() * (maxHeight - minHeight + 1)) + minHeight;

    pipes.push({
        x: canvas.width,
        top: height,
        bottom: canvas.height - height - pipeGap,
        passed: false
    });
}

function resetGame() {
    bird.y = 150;
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
    bird.y += bird.velocity;

    // Проверка столкновения с полом и потолком
    if (bird.y + bird.h >= canvas.height || bird.y <= 0) {
        isGameOver = true;
    }

    if (frameCount % 100) spawnPipe();

    for (let i = pipes.length - 1; i >= 0; i++) {
        pipes[i].x -= pipeSpeed;

        // Проверка столкновения с трубами
        if (bird.x < pipes[i].x + pipeWidth &&
            bird.x + bird.w > pipes[i].x &&
            (bird.y < pipes[i].top || bird.y + bird.h > canvas.height - pipes[i].bottom)) {
                isGameOver = true;
        }
        
        // Подсчет очков
        if (!pipes[i].passed && pipes[i].x + pipeWidth < bird.x) {
            score++;
            pipes[i].passed = true;
        }

        // Удаление труб за экраном
        if (pipes[i].x + pipeWidth < 0) pipes.splice(i, 1);
    }
}

function draw() {
    // Очистка экрана
    ctx.clearRect(0,0, canvas.width, canvas.height);

    // Рисуем птичку
    ctx.fillStyle = "#ffeb3b";
    ctx.fillRect(bird.x, bird.y, bird.w, bird.h);

    // Рисуем трубы
    ctx.fillStyle = "#4caf50";
    for (let pipe of pipes) {
        ctx.fillRect(pipe.x, 0, pipeWidth, pipe.top);
        ctx.fillRect(pipe.x, canvas.height - pipe.bottom, pipeWidth, pipe.bottom);
    }

    // Рисуем счет
    ctx.fillStyle = "#ffffff";
    ctx.font = "24px Arial";
    ctx.fillText("Score: ${score}", 15, 35);

    // Экран Game over
    if (isGameOver) {
        ctx.fillStyle = "rgba(0,0,0,0.6)";
        ctx.fillRect(0,0, canvas.width, canvas.height);

        ctx.fillStyle = "#ffffff";
        ctx.font = "30px Arial";
        ctx.textAlign = "center";
        ctx.fillText("Game Over", canvas.width / 2, canvas.height / 2 + 20);

        ctx.font = "16px Arial";
        ctx.fillText("Your result: ${score}", canvas.width / 2, canvas.height / 2 + 20);
        ctx.fillText("Tap the screen", canvas.width / 2, canvas.height / 2 + 60);
        ctx.fillText("to save the score", canvas.width / 2, canvas.height / 2 + 80);
        ctx.textAlign = "ledt"; // сброс
    }
}

function gameLoop() {
    update();
    draw();
    requestAnimationFrame(gameLoop);
}

gameLoop();