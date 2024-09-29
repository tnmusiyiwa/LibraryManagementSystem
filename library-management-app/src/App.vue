<template>
  <div class="min-h-screen bg-gray-100">
    <header class="bg-blue-600 text-white p-4">
      <div class="container mx-auto flex justify-between items-center">
        <h1 class="text-2xl font-bold">Library Management System</h1>
        <nav>
          <router-link to="/" class="mr-4">Home</router-link>
          <router-link to="/books" class="mr-4">Books</router-link>
          <template v-if="isAuthenticated">
            <router-link to="/dashboard" class="mr-4">Dashboard</router-link>
            <template v-if="isAdmin">
              <router-link :to="{ name: 'AdminUsers' }" class="mr-4">Admin</router-link>
            </template>
            <button
              @click="logout"
              class="bg-red-500 hover:bg-red-700 text-white font-bold py-2 px-4 rounded"
            >
              Logout
            </button>
          </template>
          <template v-else>
            <router-link
              to="/login"
              class="bg-green-500 hover:bg-green-700 text-white font-bold py-2 px-4 rounded"
            >
              Login
            </router-link>
          </template>
        </nav>
      </div>
    </header>

    <main class="container mx-auto p-4">
      <router-view></router-view>
    </main>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from './stores/auth'

const router = useRouter()
const authStore = useAuthStore()

const isAuthenticated = computed(() => authStore.isAuthenticated)
const isAdmin = computed(() => authStore.isAdmin)

const logout = async () => {
  await authStore.logout()
  router.push('/')
}
</script>
