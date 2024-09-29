import { defineStore } from 'pinia'
import api from '../services/api'

export const useAuthStore = defineStore('auth', {
  state: () => ({
    user: null,
    token: localStorage.getItem('token') || null
  }),
  getters: {
    isAuthenticated: (state) => !!state.token,
    isAdmin: (state) => state.user?.roles.includes('Admin')
  },
  actions: {
    async login(email, password) {
      try {
        const response = await api.post('/users/login', { email, password })
        this.token = response.data.token
        localStorage.setItem('token', this.token)
        api.defaults.headers.common['Authorization'] = `Bearer ${this.token}`
        await this.fetchUserInfo()
      } catch (error) {
        console.error('Login failed:', error)
        throw error
      }
    },
    async logout() {
      this.user = null
      this.token = null
      localStorage.removeItem('token')
      delete api.defaults.headers.common['Authorization']
    },
    async fetchUserInfo() {
      try {
        const response = await api.get('/users/me')
        this.user = response.data
      } catch (error) {
        console.error('Failed to fetch user info:', error)
        throw error
      }
    },
    async checkAuth() {
      if (this.token) {
        api.defaults.headers.common['Authorization'] = `Bearer ${this.token}`
        try {
          await this.fetchUserInfo()
        } catch (error) {
          await this.logout()
        }
      }
    }
  }
})
