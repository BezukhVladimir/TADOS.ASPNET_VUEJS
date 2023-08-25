<template>
	<div class="city">
		<div>
			<div><strong>Country:</strong> {{ city.countryName }}</div>
			<div><strong>Name:</strong> {{ city.cityName }}</div>
		</div>
		<div class="city_buttons">
			<my-button
				@click="$emit('delete', city)"
			>
				Delete
			</my-button>
		</div>
	</div>
</template>

<script>
export default {
	props: {
		city: {
			type: Object,
      required: true
		}
	}
}
</script>

<style scoped>
	.city {
		padding: 15px;
		border: 2px solid dodgerblue;
		max-width: 490px;
		margin-top: 15px;
		display: flex;
		align-items: center;
		justify-content: space-between;
	}
</style>