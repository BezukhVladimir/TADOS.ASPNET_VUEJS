<template>
	<div class="author">
		<div>
			<div><strong>Email:</strong> {{ author.email }}</div>
			<div><strong>Average rate:</strong> {{ author.averageRate }}</div>
		</div>
	</div>
</template>

<script>
export default {
	props: {
		author: {
			type: Object,
      required: true
		}
	}
}
</script>

<style scoped>
	.author {
		padding: 15px;
		border: 2px solid dodgerblue;
		max-width: 490px;
		margin-top: 15px;
		display: flex;
		align-items: center;
		justify-content: space-between;
	}
</style>