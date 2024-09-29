<template>
  <div class="flex items-center justify-center h-screen">
    <p class="text-xl">Processing login, please wait...</p>
  </div>
</template>

<script setup>
import { onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'

const router = useRouter()
const authStore = useAuthStore()

onMounted(async () => {
  try {
    await authStore.handleCallback()
    router.push('/dashboard')
  } catch (error) {
    console.error('Login failed:', error)
    router.push('/login')
  }
})
</script>