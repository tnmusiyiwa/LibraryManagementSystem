<template>
  <div class="dashboard">
    <h1 class="text-3xl font-bold mb-6">User Dashboard</h1>
    <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
      <div class="bg-white shadow rounded-lg p-6">
        <h2 class="text-2xl font-bold mb-4">Borrowed Books</h2>
        <BorrowedBooks />
      </div>
      <div class="bg-white shadow rounded-lg p-6">
        <h2 class="text-2xl font-bold mb-4">Reservations</h2>
        <Reservations />
      </div>
    </div>
    <div class="mt-6 bg-white shadow rounded-lg p-6">
      <Notifications />
    </div>
  </div>
</template>

<script setup>
import BorrowedBooks from '../components/BorrowedBooks.vue'
import Reservations from '../components/BookReservations.vue'
import Notifications from '../components/AlertNotifications.vue'
</script>
