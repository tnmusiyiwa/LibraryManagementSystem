<template>
  <div>
    <h2 class="text-2xl font-bold mb-4">Book List</h2>
    <div v-if="isAdmin" class="mb-4">
      <button
        @click="showAddBookForm = true"
        class="bg-green-500 hover:bg-green-700 text-white font-bold py-2 px-4 rounded"
      >
        Add New Book
      </button>
    </div>
    <div v-if="showAddBookForm">
      <BookForm @submit="addBook" @cancel="showAddBookForm = false" />
    </div>
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
      <div v-for="book in books" :key="book.id" class="bg-white shadow-md rounded-lg p-4">
        <h3 class="text-xl font-semibold mb-2">{{ book.title }}</h3>
        <p><strong>Author:</strong> {{ book.author }}</p>
        <p><strong>Year:</strong> {{ book.publicationYear }}</p>
        <p><strong>Category:</strong> {{ book.category }}</p>
        <p><strong>Status:</strong> {{ book.isAvailable ? 'Available' : 'Not Available' }}</p>
        <div class="mt-4 space-y-2">
          <button
            v-if="!isAdmin && book.isAvailable"
            @click="reserve(book.id)"
            class="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded w-full"
          >
            Reserve
          </button>
          <button
            v-if="!isAdmin && book.isAvailable"
            @click="showBorrowForm(book.id)"
            class="bg-green-500 hover:bg-green-700 text-white font-bold py-2 px-4 rounded w-full"
          >
            Borrow
          </button>
          <button
            v-if="!isAdmin && !book.isAvailable"
            @click="returnBook(book.id)"
            class="bg-yellow-500 hover:bg-yellow-700 text-white font-bold py-2 px-4 rounded w-full"
          >
            Return
          </button>
          <button
            v-if="isAdmin"
            @click="showUpdateBookForm(book)"
            class="bg-yellow-500 hover:bg-yellow-700 text-white font-bold py-2 px-4 rounded w-full"
          >
            Update
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
            @click="borrow"
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
    <div v-if="showUpdateForm">
      <BookForm :book="selectedBook" @submit="updateBook" @cancel="showUpdateForm = false" />
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import BookForm from './BookForm.vue'

const props = defineProps(['books', 'isAdmin'])
const emit = defineEmits(['reserve', 'borrow', 'return', 'add', 'update'])

const showAddBookForm = ref(false)
const showBorrowForm = ref(false)
const showUpdateForm = ref(false)
const borrowDays = ref(0)
const selectedBookId = ref(null)
const selectedBook = ref(null)

const reserve = (bookId) => {
  emit('reserve', bookId)
}

const openBorrowForm = (bookId) => {
  selectedBookId.value = bookId
  showBorrowForm.value = true
}

const borrow = () => {
  emit('borrow', selectedBookId.value, borrowDays.value)
  showBorrowForm.value = false
  borrowDays.value = 0
}

const returnBook = (bookId) => {
  emit('return', bookId)
}

const addBook = (newBook) => {
  emit('add', newBook)
  showAddBookForm.value = false
}

const showUpdateBookForm = (book) => {
  selectedBook.value = { ...book }
  showUpdateForm.value = true
}

const updateBook = (updatedBook) => {
  emit('update', updatedBook)
  showUpdateForm.value = false
}
</script>
