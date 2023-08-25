<template>
  <div v-if="users.length > 0">
    <h3>List of users</h3>
    <user-item v-for="user in users"
               :user="user"
               :key="user.userId"
               @delete="$emit('delete', user)" />
  </div>
  <h2 v-else style="color: red">
    List of users is empty
  </h2>
</template>

<script>
  import UserItem from "@/components/Users/UserItem";

  export default {
    components: {
      UserItem
    },
    props: {
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