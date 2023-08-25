<template>
	<div class="content">
		<div>
			<div><strong>Name:</strong> {{ content.name }}</div>
			<div><strong>Category:</strong> {{ content.category }}</div>
			<div><strong>Average rate:</strong> {{ content.averageRate }}</div>
		</div>
	</div>
</template>

<script>
export default {
	props: {
		content: {
			type: Object,
      required: true
		}
	}
}
</script>

<style scoped>
	.content {
		padding: 15px;
		border: 2px solid dodgerblue;
		max-width: 490px;
		margin-top: 15px;
		display: flex;
		align-items: center;
		justify-content: space-between;
	}
</style>