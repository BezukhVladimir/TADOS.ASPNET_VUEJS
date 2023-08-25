<template>
  <div v-if="articles.length > 0">
    <h3>List of articles</h3>
    <article-item v-for="article in articles"
                  :article="article"
                  :users="users"
                  :key="article.id"
                  @delete="$emit('delete', article)" />
  </div>
  <h2 v-else style="color: red">
    List of articles is empty
  </h2>
</template>

<script>
	import ArticleItem from "@/components/Articles/ArticleItem";

  export default {
    components: {
      ArticleItem
    },
    props: {
      articles: {
        type: Array,
        required: true
      },
      users: {
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