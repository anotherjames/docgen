const path = require('path');
const ExtractTextPlugin = require("extract-text-webpack-plugin");

var options = {
    minimize: false
};

module.exports = {
  mode: 'development',
  entry: './script.js',
  output: {
    filename: 'script.js',
    path: path.resolve(__dirname, '..', 'wwwroot', 'dist')
  },
  module: {
    rules: [
      {
        test: /\.less$/,
        use: ExtractTextPlugin.extract({
          fallback: "style-loader",
          use: [
            {
              loader: 'css-loader',
              options: options
            },
            'less-loader'
          ]
        })
      },
      {
        test: /\.css$/,
        use: ExtractTextPlugin.extract({
          fallback: "style-loader",
          use: [
            {
              loader: 'css-loader',
              options: options
            }
          ]
        })
      },
      {
        test: /\.(svg|woff|woff2|ttf|eot|jpg)$/,
        use: [
          {
            loader: 'file-loader'
          }
        ]
      }
    ]
  },
  plugins: [
    new ExtractTextPlugin("styles.css"),
  ],
  watch: true
};