import { defineStore } from 'pinia'
import api from '@/services/api'

export const useUserDashboardStore = defineStore('userDashboard', {
  state: () => ({
    borrowedBooks: [],
    reservations: []
  }),
  actions: {
    async fetchBorrowedBooks() {
      try {
        const response = await api.get('/api/users/borrowed-books')
        this.borrowedBooks = response.data
      } catch (error) {
        console.error('Failed to fetch borrowed books:', error)
      }
    },
    async fetchReservations() {
      try {
        const response = await api.get('/api/users/reservations')
        this.reservations = response.data
      } catch (error) {
        console.error('Failed to fetch reservations:', error)
      }
    },
    async borrowBook(bookId, days) {
      try {
        const response = await api.post('/api/users/borrow', { bookId, days })
        this.borrowedBooks.push(response.data)
      } catch (error) {
        console.error('Failed to borrow book:', error)
        throw error
      }
    },
    async returnBook(bookId) {
      try {
        await api.post('/api/users/return', { bookId })
        this.borrowedBooks = this.borrowedBooks.filter((b) => b.id !== bookId)
      } catch (error) {
        console.error('Failed to return book:', error)
        throw error
      }
    },
    async reserveBook(bookId) {
      try {
        const response = await api.post('/api/users/reserve', { bookId })
        this.reservations.push(response.data)
      } catch (error) {
        console.error('Failed to reserve book:', error)
        throw error
      }
    },
    async cancelReservation(reservationId) {
      try {
        await api.delete(`/api/users/reservations/${reservationId}`)
        this.reservations = this.reservations.filter((r) => r.id !== reservationId)
      } catch (error) {
        console.error('Failed to cancel reservation:', error)
        throw error
      }
    }
  }
})
