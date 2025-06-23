<template>
  <div class="taskboard">
    <h1>Task Board TR</h1>
    <form @submit.prevent="addTask" class="task-form">
      <input v-model="newTask" placeholder="Nueva tarea..." />
      <button type="submit" :disabled="!newTask.trim()">＋</button>
    </form>
    <ul>
      <li v-for="task in tasks" :key="task.id" :class="{ completed: task.isCompleted }">
        <label class="checkbox-container">
          <input type="checkbox" v-model="task.isCompleted" @change="updateTask(task)" />
          <span class="checkmark"></span>
        </label>
        <span class="task-title">{{ task.title }}</span>
        <button class="delete-btn" @click="deleteTask(task.id)" title="Eliminar">✕</button>
      </li>
    </ul>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import axios from 'axios'
import * as signalR from '@microsoft/signalr'

const tasks = ref([])
const newTask = ref('')

const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL + '/api'
})

const loadTasks = async () => {
  const res = await api.get('/tasks')
  tasks.value = res.data
}

const addTask = async () => {
  if (!newTask.value.trim()) return
  await api.post('/tasks', {
    title: newTask.value,
    isCompleted: false,
  })
  newTask.value = ''
}

const updateTask = async (task) => {
  await api.put(`/tasks/${task.id}`, task)
}

const deleteTask = async (id) => {
  await api.delete(`/tasks/${id}`)
}

onMounted(async () => {
  await loadTasks()

  const connection = new signalR.HubConnectionBuilder()
      .withUrl(import.meta.env.VITE_API_URL + '/taskhub')
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Information)
      .build()

  connection.on('TaskAdded', (task) => {
    tasks.value.push(task)
  })

  connection.on('TaskUpdated', (updated) => {
    const index = tasks.value.findIndex(t => t.id === updated.id)
    if (index >= 0) tasks.value[index] = updated
  })

  connection.on('TaskDeleted', (id) => {
    tasks.value = tasks.value.filter(t => t.id !== id)
  })

  try {
    await connection.start()
  } catch (err) {
    console.error('❌ Error al conectar SignalR:', err)
  }
})
</script>

<style scoped>
:root {
  --primary: #5b4636;
  --accent: #a67c52;
  --bg: #f8f5f0;
  --white: #fffaf3;
  --gray: #d6cfc2;
  --success: #e6e2d3;
}

.taskboard {
  max-width: 400px;
  margin: 40px auto;
  padding: 32px 24px;
  background: var(--bg);
  border-radius: 18px;
  box-shadow: 0 4px 24px 0 #0001;
  font-family: 'Inter', sans-serif;
}

h1 {
  text-align: center;
  color: var(--primary);
  font-weight: 700;
  margin-bottom: 24px;
  letter-spacing: 1px;
}

.task-form {
  display: flex;
  gap: 8px;
  margin-bottom: 24px;
}

input[type="text"], input[type="search"] {
  flex: 1;
  padding: 10px 14px;
  border: none;
  border-radius: 8px;
  background: var(--white);
  font-size: 1rem;
  outline: none;
  box-shadow: 0 1px 4px #0001;
  transition: box-shadow 0.2s;
}

input[type="text"]:focus {
  box-shadow: 0 2px 8px #4a4e6922;
}

button[type="submit"] {
  background: var(--accent);
  color: var(--white);
  border: none;
  border-radius: 8px;
  padding: 0 16px;
  font-size: 1.3rem;
  cursor: pointer;
  transition: background 0.2s;
  font-weight: 600;
  box-shadow: 0 1px 4px #0001;
}

button[type="submit"]:disabled {
  background: var(--gray);
  cursor: not-allowed;
}

ul {
  list-style: none;
  padding: 0;
  margin: 0;
}

li {
  display: flex;
  align-items: center;
  background: var(--white);
  border-radius: 8px;
  margin-bottom: 12px;
  padding: 10px 14px;
  box-shadow: 0 1px 4px #0001;
  transition: background 0.2s;
}

li.completed {
  background: var(--success);
  opacity: 0.7;
}

.checkbox-container {
  display: flex;
  align-items: center;
  margin-right: 12px;
  position: relative;
}

.checkbox-container input[type="checkbox"] {
  opacity: 0;
  width: 0;
  height: 0;
}

.checkmark {
  width: 20px;
  height: 20px;
  background: var(--bg);
  border: 2px solid var(--accent);
  border-radius: 6px;
  display: inline-block;
  position: relative;
  transition: background 0.2s;
}

.checkbox-container input:checked + .checkmark {
  background: var(--accent);
}

.checkmark:after {
  content: "";
  position: absolute;
  display: none;
}

.checkbox-container input:checked + .checkmark:after {
  display: block;
}

.checkbox-container .checkmark:after {
  left: 6px;
  top: 2px;
  width: 6px;
  height: 12px;
  border: solid var(--white);
  border-width: 0 2px 2px 0;
  transform: rotate(45deg);
  position: absolute;
}

.task-title {
  flex: 1;
  font-size: 1rem;
  color: var(--primary);
  margin-right: 10px;
  word-break: break-word;
  text-decoration: none;
  transition: color 0.2s;
}

li.completed .task-title {
  text-decoration: line-through;
  color: var(--accent);
}

.delete-btn {
  background: none;
  border: none;
  color: var(--accent);
  font-size: 1.1rem;
  cursor: pointer;
  border-radius: 50%;
  width: 28px;
  height: 28px;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: background 0.2s, color 0.2s;
}

.delete-btn:hover {
  background: #f8d7da;
  color: #b71c1c;
}
</style>