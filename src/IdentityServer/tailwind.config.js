/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
        "./Pages/**/*.{cshtml,html,razor}",
        "./Views/**/*.{cshtml,html,razor}",
        "./Areas/**/*.{cshtml,html,razor}",
        "./wwwroot/js/**/*.{js,ts}",
        "./**/*.cshtml"
    ],
    theme: {
        extend: {},
    },
    plugins: [
        require("daisyui")
    ],
    daisyui: {
        themes: ["night"]
    }
}