const path = require('path');
const ExtractTextPlugin = require("extract-text-webpack-plugin");

var stylesCss = new ExtractTextPlugin("[name].css");

var options = {
    minimize: false
};

module.exports = {
  mode: 'development',
  entry: {
      prince: './prince.scss',
      web: './web.scss'
  },
  output: {
    path: path.resolve(__dirname, '..', 'Resources', 'wwwroot', 'dist')
  },
  module: {
    rules: [
      {
        test: /\.scss/,
        use: stylesCss.extract({
          fallback: "style-loader",
          use: [
            {
              loader: 'css-loader',
              options: options
            },
            'sass-loader'
          ]
        })
      },
      {
        test: /\.css$/,
        use: stylesCss.extract({
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
      stylesCss
  ]
};