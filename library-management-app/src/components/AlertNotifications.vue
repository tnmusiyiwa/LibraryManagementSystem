<template>
  <div class="notifications">
    <h2 class="text-2xl font-bold mb-4">Notifications</h2>
    <ul v-if="notifications.length > 0" class="space-y-4">
      <li
        v-for="notification in notifications"
        :key="notification.id"
        class="bg-white shadow rounded-lg p-4"
      >
        <p class="text-gray-800">{{ notification.message }}</p>
        <p class="text-sm text-gray-500 mt-2">
          {{ new Date(notification.createdDate).toLocaleString() }}
        </p>
        <button
          @click="deleteNotification(notification.id)"
          class="mt-2 text-red-600 hover:text-red-800"
          aria-label="Delete notification"
        >
          Delete
        </button>
      </li>
    </ul>
    <p v-else class="text-gray-500">No notifications</p>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import api from '../services/api'
import { useNotification } from '../composables/useNotification'

const notifications = ref([])
const { showNotification } = useNotification()

const fetchNotifications = async () => {
  try {
    const response = await api.get('/notifications')
    notifications.value = response.data
  } catch (error) {
    console.error('Failed to fetch notifications:', error)
    showNotification('Failed to fetch notifications', 'error')
  }
}

const deleteNotification = async (id) => {
  try {
    await api.delete(`/notifications/${id}`)
    notifications.value = notifications.value.filter((n) => n.id !== id)
    showNotification('Notification deleted', 'success')
  } catch (error) {
    console.error('Failed to delete notification:', error)
    showNotification('Failed to delete notification', 'error')
  }
}

onMounted(fetchNotifications)
</script>
