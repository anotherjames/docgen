package main

import (
	"fmt"

	"github.com/pauldotknopf/docgen/hugo"
)

func main() {
	err := hugo.Build("/home/pknopf/git/test-pages")
	if err != nil {
		fmt.Println(err.Error())
	}
}
