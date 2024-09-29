<template>
  <div class="flex items-center justify-center h-screen">
    <p class="text-xl">Processing login, please wait...</p>
  </div>
</template>

<script setup>
import { onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useAuthStore } from '../stores/auth'

const router = useRouter()
const route = useRoute()
const authStore = useAuthStore()

onMounted(async () => {
  const token = route.query.token
  if (token) {
    await authStore.handleOAuthCallback(token)
    router.push('/dashboard')
  } else {
    console.error('No token found in the callback URL')
    router.push('/login')
  }
})
</script>
