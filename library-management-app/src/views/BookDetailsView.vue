<template>
  <div v-if="book">
    <h2 class="text-2xl font-bold mb-4">{{ book.title }}</h2>
    <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
      <div>
        <img :src="book.coverImage" :alt="book.title" class="w-full h-auto rounded-lg shadow-md" />
      </div>
      <div>
        <p><strong>Author:</strong> {{ book.author }}</p>
        <p><strong>Year:</strong> {{ book.publicationYear }}</p>
        <p><strong>Category:</strong> {{ book.category }}</p>
        <p><strong>ISBN:</strong> {{ book.isbn }}</p>
        <p><strong>Status:</strong> {{ book.isAvailable ? 'Available' : 'Not Available' }}</p>
        <p class="mt-4"><strong>Description:</strong></p>
        <p>{{ book.description }}</p>
        <div class="mt-4 space-x-2">
          <button
            v-if="book.isAvailable"
            @click="reserveBook"
            class="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded"
          >
            Reserve
          </button>
          <button
            v-if="book.isAvailable"
            @click="showBorrowForm = true"
            class="bg-green-500 hover:bg-green-700 text-white font-bold py-2 px-4 rounded"
          >
            Borrow
          </button>
        </div>
      </div>
    </div>
    <div
      v-if="showBorrowForm"
      class="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full"
    >
      <div class="relative top-20 mx-auto p-5 border w-96 shadow-lg rounded-md bg-white">
        <h3 class="text-lg font-bold mb-4">Borrow Book</h3>
        <input
          v-model="borrowDays"
          type="number"
          placeholder="Number of days"
          class="w-full p-2 mb-4 border rounded"
        />
        <div class="flex justify-end">
          <button
            @click="borrowBook"
            class="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded mr-2"
          >
            Borrow
          </button>
          <button
            @click="showBorrowForm = false"
            class="bg-gray-500 hover:bg-gray-700 text-white font-bold py-2 px-4 rounded"
          >
            Cancel
          </button>
        </div>
      </div>
    </div>
  </div>
  <div v-else>
    <p>Loading book details...</p>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { useBookStore } from '../stores/books'
import { useUserDashboardStore } from '../stores/userDashboard'

const route = useRoute()
const bookStore = useBookStore()
const userDashboardStore = useUserDashboardStore()

const book = ref(null)
const showBorrowForm = ref(false)
const borrowDays = ref(1)

onMounted(async () => {
  const bookId = parseInt(route.params.id)
  book.value = await bookStore.fetchBookById(bookId)
})

const reserveBook = async () => {
  try {
    await userDashboardStore.reserveBook(book.value.id)
    book.value.isAvailable = false
  } catch (error) {
    console.error('Failed to reserve book:', error)
    // Handle reservation error (e.g., show error message)
  }
}

const borrowBook = async () => {
  try {
    await userDashboardStore.borrowBook(book.value.id, borrowDays.value)
    book.value.isAvailable = false
    showBorrowForm.value = false
  } catch (error) {
    console.error('Failed to borrow book:', error)
    // Handle borrowing error (e.g., show error message)
  }
}
</script>
