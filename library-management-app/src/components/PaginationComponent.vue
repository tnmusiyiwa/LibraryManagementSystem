<template>
  <nav aria-label="Page navigation example">
    <ul class="pagination">
      <li class="page-item" :class="{ disabled: currentPage === 1 }">
        <a class="page-link" href="#" @click="handlePageChange(currentPage - 1)">Previous</a>
      </li>
      <li
        class="page-item"
        :class="{ active: page === currentPage }"
        v-for="page in pageNumbers"
        :key="page"
      >
        <a class="page-link" href="#" @click="handlePageChange(page)">{{ page }}</a>
      </li>
      <li class="page-item" :class="{ disabled: currentPage === totalPages }">
        <a class="page-link" href="#" @click="handlePageChange(currentPage + 1)">Next</a>
      </li>
    </ul>
  </nav>
</template>

<script>
export default {
  props: {
    currentPage: {
      type: Number,
      required: true
    },
    totalPages: {
      type: Number,
      required: true
    }
  },
  computed: {
    pageNumbers() {
      const totalPages = this.totalPages
      const currentPage = this.currentPage
      const visiblePages = 5 // Adjust the number of visible pages as needed

      if (totalPages <= visiblePages) {
        return Array.from({ length: totalPages }, (_, i) => i + 1)
      }

      const startPage = Math.max(currentPage - Math.floor(visiblePages / 2), 1)
      const endPage = Math.min(startPage + visiblePages - 1, totalPages)

      return Array.from({ length: endPage - startPage + 1 }, (_, i) => startPage + i)
    }
  },
  methods: {
    handlePageChange(page) {
      this.$emit('page-change', page)
    }
  }
}
</script>

<style scoped>
.pagination {
  display: flex;
  justify-content: center;
  align-items: center;
  margin-top: 1.5rem;
}

.page-item {
  margin: 0 5px;
}

.page-link {
  padding: 0.5rem 0.75rem;
  border: 1px solid #dee2e6;
  border-radius: 0.25rem;
  color: #007bff;
  text-decoration: none;
}

.page-link:hover {
  text-decoration: underline;
}

.page-item.active .page-link {
  background-color: #007bff;
  border-color: #007bff;
  color: #fff;
}

.page-item.disabled .page-link {
  color: #6c757d;
  cursor: auto;
}
</style>
