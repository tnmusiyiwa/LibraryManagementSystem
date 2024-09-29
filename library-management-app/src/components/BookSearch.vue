<template>
  <div class="mb-4">
    <div class="flex items-center border-b-2 border-blue-500 py-2">
      <Search class="text-blue-500 mr-2" />
      <input
        class="appearance-none bg-transparent border-none w-full text-gray-700 mr-3 py-1 px-2 leading-tight focus:outline-none"
        type="text"
        placeholder="Search by title, author, year, or category"
        v-model="query"
        @input="search"
      />
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { Search } from 'lucide-vue-next'

const query = ref('')
const emit = defineEmits(['search'])

const search = () => {
  emit('search', query.value)
}
</script>
