<template>
	<button class="button">
		<slot></slot>
	</button>
</template>

<script>
export default {
	name: "my-button"
}
</script>

<style scoped>
.button {
	min-width: 80px;
	padding: 10px 15px;
	background: none;
	color: dodgerblue;
	border: 2px solid dodgerblue;
}
</style>