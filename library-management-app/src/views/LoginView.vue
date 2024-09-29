<template>
  <div class="max-w-md mx-auto">
    <h2 class="text-2xl font-bold mb-4">Login</h2>
    <form @submit.prevent="handleLogin" class="space-y-4">
      <div>
        <label for="email" class="block mb-1">Email</label>
        <input v-model="email" type="email" id="email" required class="w-full p-2 border rounded" />
      </div>
      <div>
        <label for="password" class="block mb-1">Password</label>
        <input
          v-model="password"
          type="password"
          id="password"
          required
          class="w-full p-2 border rounded"
        />
      </div>
      <button
        type="submit"
        class="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded w-full"
      >
        Login
      </button>
    </form>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import { useNotification } from '../composables/useNotification'

const router = useRouter()
const authStore = useAuthStore()
const { showNotification } = useNotification()

const email = ref('')
const password = ref('')

const handleLogin = async () => {
  try {
    await authStore.login(email.value, password.value)
    showNotification('Login successful', 'success')
    router.push('/dashboard')
  } catch (error) {
    console.error('Login failed:', error)
    showNotification('Login failed. Please check your credentials.', 'error')
  }
}
</script>
