<template>
	<div class="user">
		<div>
			<div><strong>Country:</strong> {{ user.countryName }}</div>
			<div><strong>City:</strong> {{ user.cityName }}</div>
			<div><strong>Email:</strong> {{ user.userEmail }}</div>
		</div>
		<div class="user_buttons">
			<my-button
				@click="$emit('delete', user)"
			>
				Delete
			</my-button>
		</div>
	</div>
</template>

<script>
export default {
	props: {
		user: {
			type: Object,
      required: true
		}
	}
}
</script>

<style scoped>
	.user {
		padding: 15px;
		border: 2px solid dodgerblue;
		max-width: 490px;
		margin-top: 15px;
		display: flex;
		align-items: center;
		justify-content: space-between;
	}
</style>