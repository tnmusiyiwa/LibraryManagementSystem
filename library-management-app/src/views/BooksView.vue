<template>
  <div>
    <h2 class="text-2xl font-bold mb-4">Book Catalog</h2>
    <BookSearch @search="handleSearch" />
    <BookList :books="paginatedBooks" @reserve="reserveBook" @borrow="borrowBook" />
    <Pagination
      :current-page="currentPage"
      :total-pages="totalPages"
      @page-change="handlePageChange"
    />
  </div>
</template>

<script setup>
import { onMounted, computed } from 'vue'
import { useBookStore } from '../stores/books'
import { useUserDashboardStore } from '../stores/userDashboard'
import BookSearch from '../components/BookSearch.vue'
import BookList from '../components/BookList.vue'
import Pagination from '../components/PaginationComponent.vue'

const bookStore = useBookStore()
const userDashboardStore = useUserDashboardStore()

const paginatedBooks = computed(() => bookStore.paginatedBooks)
const currentPage = computed(() => bookStore.currentPage)
const totalPages = computed(() => Math.ceil(bookStore.totalBooks / bookStore.itemsPerPage))

onMounted(() => {
  bookStore.fetchBooks()
})

const handleSearch = (query) => {
  bookStore.setSearchQuery(query)
}

const handlePageChange = (page) => {
  bookStore.setPage(page)
}

const reserveBook = async (bookId) => {
  try {
    await userDashboardStore.reserveBook(bookId)
    // Update book availability in the book store
    await bookStore.fetchBooks()
  } catch (error) {
    console.error('Failed to reserve book:', error)
    // Handle reservation error (e.g., show error message)
  }
}

const borrowBook = async (bookId, days) => {
  try {
    await userDashboardStore.borrowBook(bookId, days)
    // Update book availability in the book store
    await bookStore.fetchBooks()
  } catch (error) {
    console.error('Failed to borrow book:', error)
    // Handle borrowing error (e.g., show error message)
  }
}
</script>
