<template>
  <div class="admin-notifications">
    <h1 class="text-3xl font-bold mb-6">Notification Management</h1>
    <div class="bg-white shadow rounded-lg p-6">
      <h2 class="text-2xl font-bold mb-4">Unsent Notifications</h2>
      <ul>
        <li
          v-for="notification in unsentNotifications"
          :key="notification.id"
          class="mb-4 p-4 border rounded"
        >
          <p><strong>User:</strong> {{ notification.user.name }}</p>
          <p><strong>Message:</strong> {{ notification.message }}</p>
          <p><strong>Created:</strong> {{ formatDate(notification.createdDate) }}</p>
          <button
            @click="sendNotification(notification.id)"
            class="bg-green-500 hover:bg-green-700 text-white font-bold py-1 px-2 rounded mt-2"
          >
            Send Notification
          </button>
        </li>
      </ul>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useNotification } from '../composables/useNotification'
import api from '../services/api'

const { showNotification } = useNotification()

const unsentNotifications = ref([])

const fetchUnsentNotifications = async () => {
  try {
    const response = await api.get('/admin/unsent-notifications')
    unsentNotifications.value = response.data
  } catch (error) {
    console.error('Failed to fetch unsent notifications:', error)
    showNotification('Failed to fetch unsent notifications', 'error')
  }
}

const sendNotification = async (notificationId) => {
  try {
    await api.post(`/notifications/${notificationId}/mark-as-sent`)
    showNotification('Notification sent successfully', 'success')
    await fetchUnsentNotifications()
  } catch (error) {
    console.error('Failed to send notification:', error)
    showNotification('Failed to send notification', 'error')
  }
}

const formatDate = (dateString) => {
  return new Date(dateString).toLocaleString()
}

onMounted(fetchUnsentNotifications)
</script>
