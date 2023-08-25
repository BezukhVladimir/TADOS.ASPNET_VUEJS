<template>
  <div v-if="authors.length > 0">
    <h3>List of authors</h3>
    <author-item v-for="author in authors"
                :author="author"
                :key="author.id"/>
  </div>
  <h2 v-else style="color: red">
    List of authors is empty
  </h2>
</template>

<script>
	import AuthorItem from "@/components/Leaderboards/Authors/AuthorItem";

  export default {
    components: {
      AuthorItem
    },
    props: {
      authors: {
        type: Array,
        required: true
      }
    }
  }
</script>

<style scoped>
	* {
	  margin-top: 15px;
	}
</style>  