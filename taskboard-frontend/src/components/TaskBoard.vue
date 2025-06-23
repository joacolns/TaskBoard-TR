<template>
  <div class="taskboard">
    <h1>📝 Task Board</h1>

    <form @submit.prevent="addTask">
      <input v-model="newTask" placeholder="Nueva tarea..." />
      <button type="submit">Agregar</button>
    </form>

    <ul>
      <li v-for="task in tasks" :key="task.id">
        <input type="checkbox" v-model="task.isCompleted" @change="updateTask(task)" />
        <span :style="{ textDecoration: task.isCompleted ? 'line-through' : 'none' }">
          {{ task.title }}
        </span>
        <button @click="deleteTask(task.id)">❌</button>
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

// SignalR en tiempo real
onMounted(async () => {
  await loadTasks()

  const connection = new signalR.HubConnectionBuilder()
      .withUrl(import.meta.env.VITE_API_URL + '/taskhub') // Asegurate que apunte al backend
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Information)
      .build()

  connection.on('TaskAdded', (task) => {
    console.log('🟢 TaskAdded recibido', task)
    tasks.value.push(task)
  })

  connection.on('TaskUpdated', (updated) => {
    console.log('🟢 TaskUpdated recibido', updated)
    const index = tasks.value.findIndex(t => t.id === updated.id)
    if (index >= 0) tasks.value[index] = updated
  })

  connection.on('TaskDeleted', (id) => {
    console.log('🟢 TaskDeleted recibido', id)
    tasks.value = tasks.value.filter(t => t.id !== id)
  })

  try {
    await connection.start()
    console.log('✅ Conectado a SignalR')
  } catch (err) {
    console.error('❌ Error al conectar SignalR:', err)
  }
})

</script>

<style scoped>
.taskboard {
  max-width: 600px;
  margin: 30px auto;
  padding: 20px;
  font-family: sans-serif;
  border: 1px solid #ccc;
  border-radius: 10px;
}
input[type="text"] {
  width: 80%;
  padding: 8px;
  margin-right: 10px;
}
button {
  padding: 6px 10px;
}
ul {
  list-style: none;
  padding: 0;
}
li {
  display: flex;
  align-items: center;
  margin-top: 10px;
}
</style>
