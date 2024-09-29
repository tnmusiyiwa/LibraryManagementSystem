<template>
  <div class="admin-dashboard">
    <h1 class="text-3xl font-bold mb-6">Admin Dashboard</h1>
    <div class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-6">
      <router-link
        :to="{ name: 'AdminUsers' }"
        class="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded text-center"
      >
        Manage Users
      </router-link>
      <router-link
        :to="{ name: 'AdminBooks' }"
        class="bg-green-500 hover:bg-green-700 text-white font-bold py-2 px-4 rounded text-center"
      >
        Manage Books
      </router-link>
      <router-link
        :to="{ name: 'AdminNotifications' }"
        class="bg-yellow-500 hover:bg-yellow-700 text-white font-bold py-2 px-4 rounded text-center"
      >
        Manage Notifications
      </router-link>
    </div>
    <router-view></router-view>
  </div>
</template>

<script setup></script>
