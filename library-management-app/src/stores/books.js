import { defineStore } from 'pinia'
import api from '../services/api'

export const useBookStore = defineStore('books', {
  state: () => ({
    books: [],
    totalBooks: 0,
    currentPage: 1,
    itemsPerPage: 10,
    searchQuery: ''
  }),
  getters: {
    paginatedBooks: (state) => state.books
  },
  actions: {
    async fetchBooks() {
      try {
        const response = await api.get('/books', {
          params: {
            page: this.currentPage,
            pageSize: this.itemsPerPage,
            searchQuery: this.searchQuery
          }
        })
        this.books = response.data.books
        this.totalBooks = response.data.totalBooks
      } catch (error) {
        console.error('Failed to fetch books:', error)
        throw error
      }
    },
    async fetchBookById(id) {
      try {
        const response = await api.get(`/books/${id}`)
        return response.data
      } catch (error) {
        console.error('Failed to fetch book:', error)
        throw error
      }
    },
    async addBook(book) {
      try {
        const response = await api.post('/books', book)
        this.books.push(response.data)
        return response.data
      } catch (error) {
        console.error('Failed to add book:', error)
        throw error
      }
    },
    async updateBook(book) {
      try {
        const response = await api.put(`/books/${book.id}`, book)
        const index = this.books.findIndex((b) => b.id === book.id)
        if (index !== -1) {
          this.books[index] = response.data
        }
        return response.data
      } catch (error) {
        console.error('Failed to update book:', error)
        throw error
      }
    },
    async deleteBook(bookId) {
      try {
        await api.delete(`/books/${bookId}`)
        this.books = this.books.filter((b) => b.id !== bookId)
      } catch (error) {
        console.error('Failed to delete book:', error)
        throw error
      }
    },
    setSearchQuery(query) {
      this.searchQuery = query
      this.currentPage = 1
      this.fetchBooks()
    },
    setPage(page) {
      this.currentPage = page
      this.fetchBooks()
    }
  }
})
