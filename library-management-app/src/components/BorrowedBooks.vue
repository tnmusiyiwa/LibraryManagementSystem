<template>
  <div class="borrowed-books">
    <h2 class="text-2xl font-bold mb-4">Your Borrowed Books</h2>
    <div v-if="loading" class="text-center py-4">
      <div
        class="inline-block animate-spin rounded-full h-8 w-8 border-t-2 border-b-2 border-gray-900"
      ></div>
      <p class="mt-2 text-gray-600">Loading borrowed books...</p>
    </div>
    <div
      v-else-if="error"
      class="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded"
      role="alert"
    >
      <p class="font-bold">Error</p>
      <p>{{ error }}</p>
    </div>
    <ul v-else-if="borrowedBooks.length > 0" class="space-y-4">
      <li
        v-for="borrowedBook in borrowedBooks"
        :key="borrowedBook.id"
        class="bg-white shadow rounded-lg p-4"
      >
        <div class="flex justify-between items-start">
          <div>
            <h3 class="text-lg font-semibold">{{ borrowedBook.book.title }}</h3>
            <p class="text-gray-600">Author: {{ borrowedBook.book.author }}</p>
            <p class="text-sm text-gray-500">
              Borrowed on: {{ formatDate(borrowedBook.borrowDate) }}
            </p>
          </div>
          <button
            @click="returnBook(borrowedBook.id)"
            class="bg-red-500 hover:bg-red-600 text-white font-bold py-2 px-4 rounded focus:outline-none focus:shadow-outline transition duration-150 ease-in-out"
            :disabled="returning === borrowedBook.id"
          >
            {{ returning === borrowedBook.id ? 'Returning...' : 'Return Book' }}
          </button>
        </div>
      </li>
    </ul>
    <p v-else class="text-gray-600">You don't have any active borrowed books.</p>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import api from '../services/api'
import { useNotification } from '../composables/useNotification'

const borrowedBooks = ref([])
const loading = ref(true)
const error = ref(null)
const returning = ref(null)

const { showNotification } = useNotification()

const fetchBorrowedBooks = async () => {
  try {
    loading.value = true
    error.value = null
    const response = await api.get('/users/borrowed-books')
    borrowedBooks.value = response.data
  } catch (err) {
    console.error('Failed to fetch reservations:', err)
    error.value = 'Failed to load reservations. Please try again later.'
  } finally {
    loading.value = false
  }
}

const returnBook = async (id) => {
  try {
    returning.value = id
    await api.delete(`/users/return-book/${id}`)
    borrowedBooks.value = borrowedBooks.value.filter((r) => r.id !== id)
    showNotification('Borrowed book returned successfully', 'success')
  } catch (err) {
    console.error('Failed to return book:', err)
    showNotification('Failed to return book. Please try again.', 'error')
  } finally {
    returning.value = null
  }
}

const formatDate = (dateString) => {
  return new Date(dateString).toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  })
}

onMounted(fetchBorrowedBooks)
</script>
