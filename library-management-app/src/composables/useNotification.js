import { ref } from 'vue'

export function useNotification() {
  const message = ref('')
  const type = ref('')
  const show = ref(false)

  function showNotification(msg, notificationType = 'info') {
    message.value = msg
    type.value = notificationType
    show.value = true

    setTimeout(() => {
      show.value = false
    }, 5000)
  }

  return {
    message,
    type,
    show,
    showNotification
  }
}
