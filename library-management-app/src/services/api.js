import axios from 'axios'

const api = axios.create({
  baseURL: import.meta.env.VITE_APP_API_URL || 'https://localhost:7086/api'
})

api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token')
    if (token) {
      config.headers['Authorization'] = `Bearer ${token}`
    }
    return config
  },
  (error) => {
    return Promise.reject(error)
  }
)

export default api
