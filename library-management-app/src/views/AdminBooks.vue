<template>
  <div class="admin-books">
    <h1 class="text-3xl font-bold mb-6">Book Management</h1>
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
      <div class="bg-white shadow rounded-lg p-6">
        <h2 class="text-2xl font-bold mb-4">Borrowed Books</h2>
        <ul>
          <li v-for="book in borrowedBooks" :key="book.id" class="mb-2">
            {{ book.book.title }} - {{ book.user.name }}
          </li>
        </ul>
      </div>
      <div class="bg-white shadow rounded-lg p-6">
        <h2 class="text-2xl font-bold mb-4">Reserved Books</h2>
        <ul>
          <li v-for="reservation in reservedBooks" :key="reservation.id" class="mb-2">
            {{ reservation.book.title }} - {{ reservation.user.name }}
          </li>
        </ul>
      </div>
      <div class="bg-white shadow rounded-lg p-6">
        <h2 class="text-2xl font-bold mb-4">Overdue Books</h2>
        <ul>
          <li v-for="book in overdueBooks" :key="book.id" class="mb-2">
            {{ book.book.title }} - {{ book.user.name }}
          </li>
        </ul>
      </div>
      <div class="bg-white shadow rounded-lg p-6">
        <h2 class="text-2xl font-bold mb-4">Almost Due Books</h2>
        <ul>
          <li v-for="book in almostDueBooks" :key="book.id" class="mb-2">
            {{ book.book.title }} - {{ book.user.name }}
          </li>
        </ul>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useNotification } from '../composables/useNotification'
import api from '../services/api'

const { showNotification } = useNotification()

const borrowedBooks = ref([])
const reservedBooks = ref([])
const overdueBooks = ref([])
const almostDueBooks = ref([])

const fetchBookData = async () => {
  try {
    const [borrowedResponse, reservedResponse, overdueResponse, almostDueResponse] =
      await Promise.all([
        api.get('/admin/borrowed-books'),
        api.get('/admin/reserved-books'),
        api.get('/admin/overdue-books'),
        api.get('/admin/almost-due-books')
      ])

    borrowedBooks.value = borrowedResponse.data
    reservedBooks.value = reservedResponse.data
    overdueBooks.value = overdueResponse.data
    almostDueBooks.value = almostDueResponse.data
  } catch (error) {
    console.error('Failed to fetch book data:', error)
    showNotification('Failed to fetch book data', 'error')
  }
}

onMounted(fetchBookData)
</script>
