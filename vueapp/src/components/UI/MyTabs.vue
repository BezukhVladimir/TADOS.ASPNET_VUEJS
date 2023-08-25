<template>
	<div class="tabs">
		<ul class='tabs__header'>
			<li
				v-for='title in tabTitles'
				:key='title'
				:class="{ selected: title == selectedTitle }"
				@click="selectedTitle = title"
			>
				{{ title }}
			</li>
		</ul>
		<slot />
	</div>
</template>

<script>
	import { ref, provide } from 'vue'
	export default {
		name: 'my-tabs',
		setup(props, { slots }) {
			const tabTitles = ref(slots.default().map((tab) => tab.props.title))
			const selectedTitle = ref(tabTitles.value[0])

			provide('selectedTitle', selectedTitle)
			return {
				selectedTitle,
				tabTitles,
			}
		},
	}
</script>

<style scoped>
	.tabs {
		max-width: 500px;
		padding-top: 15px;
		margin: 0 left;
	}

	.tabs__header {
		margin-bottom: 15px;
		list-style: none;
		padding: 0;
		display: flex;
	}

	.tabs__header li {
		width: 235px;
		text-align: center;
		padding: 10px 15px;
		margin-right: 10px;
		background-color: #ddd;
		border-radius: 5px;
		cursor: pointer;
		transition: 0.4s all ease-out;
	}

	.tabs__header li.selected {
		background-color: dodgerblue;
		color: white;
	}
</style>