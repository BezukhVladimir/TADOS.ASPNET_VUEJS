<template>
	<input :value="modelValue" @input="updateInput" class="input" type="text">
</template>

<script>
export default {
	name: 'my-input',
	props: {
		modelValue: [String, Number]
	},
	methods: {
		updateInput(event) {
			this.$emit('update:modelValue', event.target.value)
		}
	}
}
</script>

<style scoped>
	.input {
		width: 100%;
		border: 2px solid dodgerblue;
		font-family: Helvetica, Arial, sans-serif;
		font-size: 12px;
		color: rgba(0, 0, 0, 0.6);
		padding: 10px 15px;
		margin-top: 10px;
		max-width: 490px;
	}
</style>