<template lang="html">
	<div class='tab' v-show="title == selectedTitle">
		<slot/>
	</div>
</template>

<script>
	import { inject } from 'vue'

	export default {
		name: 'my-tab',
		props: ['title'],
		setup() {
			const selectedTitle = inject('selectedTitle')

			return {
				selectedTitle,
			}
		},
	}
</script>

<style scoped>
	.tap {
		min-height: 400px;
		padding: 15px;
		border-radius: 5px;
		box-shadow: 0 3px 5px 5px rgba(0,0,0,0.05);
	}
</style>