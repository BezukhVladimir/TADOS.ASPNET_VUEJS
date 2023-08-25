<template>
	<div class="country">
		<div>
			<div><strong>Name:</strong> {{ country.name }}</div>
		</div>
		<div class="country_buttons">
			<my-button
				@click="$emit('delete', country)"
			>
				Delete
			</my-button>
		</div>
	</div>
</template>

<script>
export default {
	props: {
		country: {
			type: Object,
      required: true
		}
	}
}
</script>

<style scoped>
.country {
	padding: 15px;
	max-width: 490px;
	border: 2px solid dodgerblue;
	margin-top: 15px;
	display: flex;
	align-items: center;
	justify-content: space-between;
}
</style>